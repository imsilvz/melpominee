import { Character } from '../types/Character';

interface UpdateOptions {
  property?: string;
  debounceOptions?: {
    enable: boolean;
    delay: number;
  };
}

interface UpdateResponse {
  success?: boolean;
  error?: string;
}

// eslint-disable-next-line import/prefer-default-export
export const toTitleCase = (str: string) => {
  return str
    .toLowerCase()
    .split(' ')
    .map((s) => s.charAt(0).toUpperCase() + s.substring(1))
    .join(' ');
};

export const cleanUpdate = <T extends object>(obj: T): unknown => {
  return Object.fromEntries(
    Object.entries(obj)
      .filter(([_, v]) => v != null)
      .map(([k, v]) => [k, v === Object(v) ? cleanUpdate(v) : v]),
  );
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const isObject = (obj: any): boolean => {
  return (obj && typeof obj === 'object' && !Array.isArray(obj)) as boolean;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const deepMerge = (target: object, source: any): object => {
  let output = { ...target };
  if (isObject(target) && isObject(source)) {
    Object.keys(source as object).forEach((key) => {
      if (isObject((source as object)[key as keyof object])) {
        if (!(key in target)) {
          const val = (source as object)[key as keyof object];
          output = {
            ...output,
            [key as keyof object]: val,
          };
        } else {
          output = {
            ...output,
            [key as keyof object]: deepMerge(
              target[key as keyof object],
              (source as object)[key as keyof object],
            ),
          };
        }
      } else {
        output = {
          ...output,
          [key as keyof object]: (source as object)[key as keyof object],
        };
      }
    });
  }
  return output;
};

const handleUpdateAsync = async (
  endpoint: string,
  payload: object,
  opts?: UpdateOptions,
) => {
  const result = await fetch(endpoint, {
    method: 'PUT',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  });
  if (result.ok) {
    const responseBody = await (result.json() as Promise<UpdateResponse>);
    if (responseBody.success) {
      // success!
    } else {
      // handle error
    }
  } else {
    // handle error
  }
};

const getDebounceKey = (item: object): string => {
  let key = `${Object.keys(item).join('-')}`;
  Object.keys(item).forEach((itemKey) => {
    if (isObject(item[itemKey as keyof object])) {
      key = `${key}-${getDebounceKey(item[itemKey as keyof object])}`;
    }
  });
  return key;
};

const DebounceMap = new Map<string, ReturnType<typeof setTimeout>>();
export const handleUpdate = (
  endpoint: string,
  character: Character,
  payload: object,
  updateFunc: (value: React.SetStateAction<Character | null>) => void,
  opts?: UpdateOptions,
) => {
  // if the character is null, there is nothing to update
  if (!character) {
    return;
  }
  // clean update object of null and undefined
  const update = cleanUpdate(payload) as object;
  // are we updating header fields, or property fields?
  if (
    opts &&
    opts.property &&
    Object.prototype.hasOwnProperty.call(character, opts.property)
  ) {
    // handle property update
    updateFunc(
      (char) =>
        char && {
          ...char,
          [opts.property as keyof Character]: deepMerge(
            char[opts.property as keyof Character] as object,
            update,
          ),
        },
    );
  } else {
    // handle header update
    updateFunc((char) => char && (deepMerge(char, update) as Character));
  }
  // send network request
  if (opts?.debounceOptions?.enable) {
    const debounceKey = `${opts?.property || 'character'}-${getDebounceKey(update)}`;
    console.log(debounceKey);
    if (DebounceMap.has(debounceKey)) {
      // clear previous timer and hold updates
      clearTimeout(DebounceMap.get(debounceKey));
      DebounceMap.delete(debounceKey);
    }
    DebounceMap.set(
      debounceKey,
      setTimeout(() => {
        DebounceMap.delete(debounceKey);
        handleUpdateAsync(endpoint, payload, opts).catch(console.error);
      }, opts?.debounceOptions?.delay || 0),
    );
  } else {
    handleUpdateAsync(endpoint, payload, opts).catch(console.error);
  }
};
