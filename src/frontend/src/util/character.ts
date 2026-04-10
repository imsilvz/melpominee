import { Character } from '../types/Character';

export interface UpdateOptions {
  apiPayload?: object;
  debounceOptions?: {
    enable: boolean;
    delay: number;
  };
  property?: string;
  updateHandler?: (char: Character | null, payload: object) => Character | null;
}

export type CharacterCommandTypeString =
  | 'header'
  | 'attributes'
  | 'skills'
  | 'stats'
  | 'disciplines'
  | 'powers'
  | 'beliefs'
  | 'backgrounds'
  | 'merits'
  | 'flaws'
  | 'profile';

export interface CharacterCommandType {
  type: CharacterCommandTypeString;
  data: object;
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
  charId: number,
  commands: CharacterCommandType[],
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
  const result = await fetch(`/api/vtmv5/character/${charId}/`, {
    method: 'PUT',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      updateId,
      commands,
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
  charId: number,
  updateId: string | null,
  command: CharacterCommandType,
  updateFunc: (value: React.SetStateAction<Character | null>) => void,
  opts?: UpdateOptions,
) => {
  // if the command is null, there is nothing to update
  if (!command || !command.data) {
    return;
  }
  // if an updateId is attached,
  // check if we have already added it
  if (updateId && UpdateQueue.get(charId)?.includes(updateId)) {
    // break out of update if it has already been applied
    return;
  }
  // clean update object of null and undefined
  const update = cleanUpdate(command.data) as object;
  if (opts?.updateHandler) {
    // run custom update function if available
    updateFunc(
      (char) =>
        (char &&
          char.id === charId &&
          opts.updateHandler &&
          opts.updateHandler(char, update)) ||
        char,
    );
  } else {
    // are we updating header fields, or property fields?
    if (opts?.property) {
      // handle property update
      updateFunc(
        (char) =>
          (char &&
            char.id === charId && {
              ...char,
              [opts.property as keyof Character]: deepMerge(
                char[opts.property as keyof Character] as object,
                update,
              ),
            }) ||
          char,
      );
    } else {
      // handle header update
      updateFunc(
        (char) =>
          (char && char.id === charId && (deepMerge(char, update) as Character)) ||
          char,
      );
    }
  }
  if (updateId === null) {
    // send network request (local change, not from SignalR)
    const networkData = opts?.apiPayload || update;
    if (opts?.debounceOptions?.enable) {
      const debounceKey = `${command.type}-${getDebounceKey(update)}`;
      if (DebounceMap.has(debounceKey)) {
        // clear previous timer and hold updates
        clearTimeout(DebounceMap.get(debounceKey));
        DebounceMap.delete(debounceKey);
      }
      DebounceMap.set(
        debounceKey,
        setTimeout(() => {
          DebounceMap.delete(debounceKey);
          handleUpdateAsync(charId, [
            { type: command.type, data: networkData },
          ]).catch(console.error);
        }, opts?.debounceOptions?.delay || 0),
      );
    } else {
      handleUpdateAsync(charId, [{ type: command.type, data: networkData }]).catch(
        console.error,
      );
    }
  }
};
