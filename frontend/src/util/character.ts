import { Character } from '../types/Character';

interface UpdateOptions {
  apiPayload?: object;
  debounceOptions?: {
    enable: boolean;
    delay: number;
  };
  property?: string;
  updateHandler?: (char: Character | null, payload: object) => Character | null;
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

export const cleanUpdate = <T extends object>(obj: T): unknown => {
  return Object.fromEntries(
    Object.entries(obj)
      .filter(([_, v]) => v != null)
      .map(([k, v]) => [k, isObject(v) ? cleanUpdate(v) : v]),
  );
};

const UpdateQueue = new Map<number, string[]>();
const handleUpdateAsync = async (
  endpoint: string,
  charId: number,
  payload: object,
) => {
  // create update ID
  const updateId = crypto.randomUUID();
  if (!UpdateQueue.has(charId)) {
    UpdateQueue.set(charId, []);
  }
  const charUpdateQueue = UpdateQueue.get(charId);
  charUpdateQueue?.push(updateId);
  if ((charUpdateQueue?.length || 0) > 50) {
    charUpdateQueue?.shift();
  }

  // make request
  const result = await fetch(endpoint, {
    method: 'PUT',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      updateId,
      updateData: payload,
    }),
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
  endpoint: string | null,
  charId: number,
  updateId: string | null,
  payload: object,
  updateFunc: (value: React.SetStateAction<Character | null>) => void,
  opts?: UpdateOptions,
) => {
  // if the payload is null, there is nothing to update
  if (!payload) {
    return;
  }
  // if an updateId is attached,
  // check if we have already added it
  if (updateId && UpdateQueue.get(charId)?.includes(updateId)) {
    // break out of update if it has already been applied
    return;
  }
  // clean update object of null and undefined
  const update = cleanUpdate(payload) as object;
  console.log(payload, update);
  if (opts?.updateHandler) {
    // run custom update function if available
    updateFunc(
      (char) => (opts.updateHandler && opts.updateHandler(char, payload)) || char,
    );
  } else {
    // are we updating header fields, or property fields?
    if (opts?.property) {
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
  }
  if (endpoint) {
    // send network request
    if (opts?.debounceOptions?.enable) {
      const debounceKey = `${opts?.property || 'character'}-${getDebounceKey(
        update,
      )}`;
      if (DebounceMap.has(debounceKey)) {
        // clear previous timer and hold updates
        clearTimeout(DebounceMap.get(debounceKey));
        DebounceMap.delete(debounceKey);
      }
      DebounceMap.set(
        debounceKey,
        setTimeout(() => {
          DebounceMap.delete(debounceKey);
          handleUpdateAsync(endpoint, charId, opts?.apiPayload || payload).catch(
            console.error,
          );
        }, opts?.debounceOptions?.delay || 0),
      );
    } else {
      handleUpdateAsync(endpoint, charId, opts?.apiPayload || payload).catch(
        console.error,
      );
    }
  }
};
