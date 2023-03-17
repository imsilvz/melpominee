import React, { useEffect, useRef, useState } from 'react';

// redux
import { useAppSelector } from '../../../redux/hooks';
import {
  selectDisciplinePowers,
  selectDisciplines,
} from '../../../redux/reducers/masterdataReducer';

// types
import { Character, CharacterDisciplines } from '../../../types/Character';

// local files
import StatDots from './StatDots';
import './DisciplineSection.scss';
import CharacterSheetSection from './CharacterSheetSection';

interface PowerRowProps {
  id: string;
  level: number;
  school: string;
  power: string;
  onChange?: (oldVal: string, newVal: string) => void;
}

interface DisciplineTileProps {
  id: string;
  level: number;
  school: string;
  powers: string[];
  onLevelChange?: (oldVal: number, newVal: number) => void;
  onPowerChange?: (
    index: number,
    oldVal: string,
    newVal: string,
    schoolChange?: boolean,
  ) => void;
}

interface DisciplineSectionProps {
  characterId: number;
  levels: CharacterDisciplines;
  powers: string[];
  onLevelChange?: (school: string, oldVal: number, newVal: number) => void;
  onPowerChange?: (oldVal: string, newVal: string, schoolChange?: boolean) => void;
}

const PowerRow = ({ id, level, school, power, onChange }: PowerRowProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  return (
    <div className="charactersheet-disciplines-power">
      <select
        value={power || ''}
        disabled={
          !Object.prototype.hasOwnProperty.call(disciplines, school) || level === 0
        }
        onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
          if (onChange) {
            onChange(power, event.target.value);
          }
        }}
      >
        {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
        <option value="" />
        {Array.from(Array(level), (_skip, i) => i).map((groupLevel) => (
          <optgroup
            key={`${id}-${school}-group${groupLevel}`}
            label={`Level ${groupLevel + 1}`}
          >
            {Object.prototype.hasOwnProperty.call(disciplines, school) &&
              disciplines[school].powers
                .filter((val) => val.level === groupLevel + 1)
                .map((opt) => (
                  <option
                    key={`${id}-${school}-${opt.level}-option-${opt.id}`}
                    value={opt.id}
                  >
                    {opt.name}
                  </option>
                ))}
          </optgroup>
        ))}
      </select>
      <span />
    </div>
  );
};

const DisciplineTile = ({
  id,
  level,
  school,
  powers,
  onLevelChange,
  onPowerChange,
}: DisciplineTileProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  const disciplineList = Array.from(Object.keys(disciplines)).sort();
  return (
    <div className="charactersheet-disciplines-block">
      <div className="charactersheet-disciplines-school">
        <div className="charactersheet-disciplines-school-select">
          <select
            value={school || ''}
            onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
              // discipline category has changed!
              if (onPowerChange) {
                onPowerChange(-1, school, event.target.value, true);
              }
            }}
          >
            {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
            <option value="" />
            {disciplineList.map((discKey) => (
              <option key={`${id}-${discKey}`} value={discKey}>
                {disciplines[discKey].name}
              </option>
            ))}
          </select>
          <span />
        </div>
        <StatDots
          rootKey={`${id}-dots`}
          value={level}
          onChange={(oldVal, newVal) => {
            if (onLevelChange) {
              onLevelChange(oldVal, newVal);
            }
          }}
        />
      </div>
      {Array.from(Array(5), (_skip, i) => i).map((_skipRow, rowIdx) => (
        <PowerRow
          id={`${id}-power${rowIdx}`}
          key={`${id}-power${rowIdx}`}
          level={level}
          school={school}
          power={powers[rowIdx] || ''}
          onChange={(oldVal, newVal) => {
            // discipline power has changed
            if (onPowerChange) {
              onPowerChange(rowIdx, oldVal, newVal);
            }
          }}
        />
      ))}
    </div>
  );
};

interface DisciplineSectionTile {
  school: string;
  powers: string[];
  level: number;
}

interface DisciplineSectionLayout {
  [key: number]: DisciplineSectionTile;
}

const DisciplineSection = ({
  characterId,
  levels,
  powers,
  onLevelChange,
  onPowerChange,
}: DisciplineSectionProps) => {
  // we use both state and ref so that there is always a current reference
  // to the layout, and so we can more easily determine if it is null
  const sectionLayoutRef = useRef<DisciplineSectionLayout | null>(null);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const [sectionLayout, setSectionLayout] = useState<DisciplineSectionLayout | null>(
    null,
  );

  useEffect(() => {
    // reset previous layout
    sectionLayoutRef.current = null;
    setSectionLayout(null);
  }, [characterId]);

  useEffect(() => {
    if (!Object.keys(disciplinePowers).length) {
      // if masterdata has not loaded yet,
      // do not proceed
      return;
    }

    // maintain layout in a ref
    if (!sectionLayoutRef.current) {
      // setup initial layout
      const newSectionLayout: DisciplineSectionLayout = {};
      // get count of powers in each character discipline
      const discCountMap = powers.reduce((prev, curr) => {
        // check power is valid
        if (Object.prototype.hasOwnProperty.call(disciplinePowers, curr)) {
          const power = disciplinePowers[curr];
          if (!prev.has(power.school)) {
            prev.set(power.school, 1);
          } else {
            prev.set(power.school, (prev.get(power.school) as number) + 1);
          }
        }
        return prev;
      }, new Map<string, number>());
      // check if the player has any disciplines unaccounted for by powers
      const mappedDisciplines: string[] = Array.from(discCountMap.keys());
      Array.from(Object.keys(levels)).forEach((discId) => {
        const level = levels[discId as keyof CharacterDisciplines] as number;
        if (level > 0) {
          if (!mappedDisciplines.includes(discId)) {
            discCountMap.set(discId, 0);
          }
        }
      });
      // assuming we have 5 rows, figure out how many tiles we need for each discipline
      // then sort them in alphabetical order
      const discTileList = Array.from(discCountMap.entries())
        .reduce((output: string[], kvp) => {
          output.push(kvp[0]);
          for (let i = 1; i < Math.ceil(kvp[1] / 5); i++) {
            output.push(kvp[0]);
          }
          return output;
        }, [])
        .sort();
      // when we create the layout for the first time, it's just sequential
      // keep a map of indexes for each power
      const powerIndexMap = new Map<string, number>();
      for (let i = 0; i < discTileList.length; i++) {
        const tileLayout: DisciplineSectionTile = {
          school: discTileList[i],
          powers: [],
          level: levels[discTileList[i] as keyof CharacterDisciplines] || 0,
        };
        // loop through powers
        for (let j = 0; j < powers.length; j++) {
          const power = powers[j];
          const powerInfo = disciplinePowers[power];
          if (powerInfo.school === tileLayout.school) {
            tileLayout.powers.push(power);
            if (!powerIndexMap.has(powerInfo.school)) {
              powerIndexMap.set(powerInfo.school, 1);
            } else {
              const val = (powerIndexMap.get(powerInfo.school) as number) + 1;
              powerIndexMap.set(powerInfo.school, val);
              if (val % 5 === 0) {
                // if we have filled all five rows,
                // then we break out of the loop
                break;
              }
            }
          }
        }
        // sort powers by level, and then alphabetically
        tileLayout.powers = tileLayout.powers.sort().sort((a, b) => {
          const aInfo = disciplinePowers[a];
          const bInfo = disciplinePowers[b];
          if (aInfo.level < bInfo.level) {
            return -1;
          }
          if (aInfo.level > bInfo.level) {
            return 1;
          }
          return 0;
        });
        newSectionLayout[i] = tileLayout;
      }
      // add empty rows to fill remaining space
      const offset = Object.keys(newSectionLayout).length;
      const paddingRows = offset % 3;
      for (let i = 0; i < 3 - paddingRows; i++) {
        newSectionLayout[offset + i] = {
          school: '',
          powers: [],
          level: 0,
        };
      }
      sectionLayoutRef.current = newSectionLayout;
    } else {
      const newSectionLayout = { ...sectionLayoutRef.current };
      // first, we handle disciplines. calculate number of tiles needed!
      const countMap = new Map<string, number>();
      const tileCountMap = new Map<string, number>();
      Object.keys(levels).forEach((discipline) => {
        const level = levels[discipline as keyof CharacterDisciplines];
        if (level && level >= 1) {
          countMap.set(discipline, 0);
          // iterate powers!
          powers.forEach((power) => {
            const powerData = disciplinePowers[power];
            if (powerData.school === discipline) {
              countMap.set(discipline, (countMap.get(discipline) as number) + 1);
            }
          });
        }
      });
      // countmap now contains a count of every power where we have
      // at least one dot. dividing count by rowcount (5) and using ceil
      // will give the number of necessary tiles.
      Array.from(countMap.keys()).forEach((discipline) => {
        const tileCount = Math.ceil((countMap.get(discipline) as number) / 5);
        // if we have points in a discipline, display at least one tile with it!
        tileCountMap.set(discipline, tileCount > 0 ? tileCount : 1);
      });

      // with our tile counts, we should iterate all tiles.
      // IF the discipline does not exist in tilecount, blank out the tile
      // IF the discipline has more tiles than tilecount, remove the later ones
      let layoutKeys = Object.keys(newSectionLayout);
      layoutKeys.forEach((layoutKey) => {
        const tileInfo = newSectionLayout[parseInt(layoutKey, 10)];
        if (!tileCountMap.has(tileInfo.school)) {
          // blank it out!
          newSectionLayout[parseInt(layoutKey, 10)] = {
            school: '',
            powers: [],
            level: 0,
          };
          return;
        }
        const allotment = tileCountMap.get(tileInfo.school) as number;
        if (allotment > 0) {
          // if we still have room for more tiles,
          // reduce allotment by one and move to the next tile!
          tileCountMap.set(tileInfo.school, allotment - 1);
        } else {
          // if we have no more tile budget, and this tile has no powers in it
          if (tileInfo.powers.length <= 0) {
            // blank the tile
            newSectionLayout[parseInt(layoutKey, 10)] = {
              school: '',
              powers: [],
              level: 0,
            };
          }
        }
      });
      // next, we want to fill in empty tiles with needed ones
      // when we find an empty tile, iterate our allotment map
      // and adjust allocations accordingly
      layoutKeys = Object.keys(newSectionLayout);
      layoutKeys.forEach((layoutKey) => {
        const tileInfo = newSectionLayout[parseInt(layoutKey, 10)];
        if (tileInfo.school === '') {
          let newDiscipline = '';
          const tileCountKey = Array.from(tileCountMap.keys());
          for (let i = 0; i < tileCountKey.length; i++) {
            const allotment = tileCountMap.get(tileCountKey[i]) as number;
            if (allotment > 0) {
              newDiscipline = tileCountKey[i];
              tileCountMap.set(tileCountKey[i], allotment - 1);
              break;
            }
          }
          if (newDiscipline !== '') {
            newSectionLayout[parseInt(layoutKey, 10)] = {
              school: newDiscipline,
              powers: [],
              level: levels[newDiscipline as keyof CharacterDisciplines] || 0,
            };
          }
        }
      });
      // finally, we will iterate our allotment map to see if additional
      // tiles will be required to meet demand. if they are, create them.
      Array.from(tileCountMap.keys()).forEach((discipline) => {
        let allotment = tileCountMap.get(discipline) as number;
        while (allotment > 0) {
          newSectionLayout[Object.keys(newSectionLayout).length] = {
            school: discipline,
            powers: [],
            level: levels[discipline as keyof CharacterDisciplines] || 0,
          };
          allotment -= 1;
          tileCountMap.set(discipline, allotment);
        }
      });
      // and as a finishing touch, we will make sure there is a full row of tiles
      // IF we have a non multiple of 3, add enough cells to finish the row
      const offset = Object.keys(newSectionLayout).length;
      const paddingRows = offset % 3;
      if (paddingRows !== 0) {
        for (let i = 0; i < 3 - paddingRows; i++) {
          newSectionLayout[offset + i] = {
            school: '',
            powers: [],
            level: 0,
          };
        }
      }
      // alternatively, if we have too many empty tiles, remove a few!
      // first, loop backwards from the last tile and find index
      let firstIdx = 0;
      for (let i = Object.keys(newSectionLayout).length - 1; i >= 0; i--) {
        const tileInfo = newSectionLayout[i];
        if (tileInfo.school !== '') {
          firstIdx = i;
          break;
        }
      }
      // next, we find the end of that row using the mod operator
      // and we can adjust the row count accordingly!
      const finalRowIdx = firstIdx + (3 - ((firstIdx + 1) % 3));
      for (let i = Object.keys(newSectionLayout).length - 1; i > finalRowIdx; i--) {
        delete newSectionLayout[i];
      }
      for (let i = Object.keys(newSectionLayout).length; i <= finalRowIdx; i++) {
        newSectionLayout[i] = {
          school: '',
          powers: [],
          level: 0,
        };
      }
      // now it's time to work on powers!!! iterate all tiles and
      // get a list of whatever powers are currently assigned.
      const layoutPowers: string[] = [];
      layoutKeys = Object.keys(newSectionLayout);
      layoutKeys.forEach((layoutKey) => {
        const tileInfo = newSectionLayout[parseInt(layoutKey, 10)];
        // and while we're here, make sure to adjust levels
        if (tileInfo.school !== '') {
          tileInfo.level =
            levels[tileInfo.school as keyof CharacterDisciplines] || 0;
          layoutPowers.push(...tileInfo.powers);
        }
      });
      // create two lists
      // toAdd contains all powers that need to be added
      // toRemove contains all powers that need to be removed!
      const toAdd = powers.filter((powerId) => !layoutPowers.includes(powerId));
      const toRemove = layoutPowers.filter(
        (powerId) => powerId && !powers.includes(powerId),
      );
      // remove powers that do not belong in our list
      toRemove.forEach((removeVal) => {
        for (let i = 0; i < layoutKeys.length; i++) {
          const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
          tileInfo.powers = tileInfo.powers.filter(
            (powerId) => powerId !== removeVal,
          );
        }
      });
      // add powers that do belong in our list
      toAdd.forEach((addVal) => {
        const powerInfo = disciplinePowers[addVal];
        for (let i = 0; i < layoutKeys.length; i++) {
          const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
          if (tileInfo.school === powerInfo.school) {
            // do not insert into a tile where there is no room!
            if (tileInfo.powers.length < 5) {
              // add power,
              // then sort by level,
              // and then alphabetically
              tileInfo.powers.push(addVal);
              tileInfo.powers = tileInfo.powers.sort().sort((a, b) => {
                const aInfo = disciplinePowers[a];
                const bInfo = disciplinePowers[b];
                if (aInfo.level < bInfo.level) {
                  return -1;
                }
                if (aInfo.level > bInfo.level) {
                  return 1;
                }
                return 0;
              });
              break;
            }
          }
        }
      });
      sectionLayoutRef.current = newSectionLayout;
    }
    setSectionLayout(sectionLayoutRef.current);
  }, [disciplinePowers, characterId, levels, powers]);

  return (
    <CharacterSheetSection
      className="charactersheet-disciplines-inner"
      title="Disciplines"
    >
      {sectionLayout &&
        Object.keys(sectionLayout).map((tileIdx) => {
          const tileLayout: DisciplineSectionTile =
            sectionLayout[parseInt(tileIdx, 10)];
          return (
            <DisciplineTile
              id={`disciplines-tile${tileIdx}`}
              key={`disciplines-tile${tileIdx}`}
              level={tileLayout.level}
              school={tileLayout.school}
              powers={tileLayout.powers}
              onLevelChange={(oldVal, newVal) => {
                const currentSectionLayout = sectionLayoutRef?.current;
                if (!currentSectionLayout) {
                  // this should only happen prior to initialization
                  // but it will also protect character swaps from changes!
                  return;
                }
                // get tile info
                const tileInfo = currentSectionLayout[parseInt(tileIdx, 10)];
                if (tileInfo.school !== '') {
                  // iterate all tiles and remove any disciplines
                  // which do not meet level or amalgam requirements
                  setSectionLayout({ ...currentSectionLayout });
                  if (onLevelChange) {
                    onLevelChange(tileInfo.school, oldVal, newVal);
                  }
                }
              }}
              onPowerChange={(powerIdx, oldVal, newVal, schoolChange) => {
                const currentSectionLayout = sectionLayoutRef?.current;
                if (!currentSectionLayout) {
                  // this should only happen prior to initialization
                  // but it will also protect character swaps from changes!
                  return;
                }
                if (!schoolChange) {
                  // this is a change to discipline powers
                  const oldPowerInfo = disciplinePowers[oldVal];
                  const newPowerInfo = disciplinePowers[newVal];
                  if (newPowerInfo) {
                    // new power is valid, check if it is coming from elsewhere
                    let found = false;
                    const replaceLocation = { tile: -1, row: -1 };
                    const sectionKeys = Object.keys(currentSectionLayout);
                    for (let i = 0; i < sectionKeys.length; i++) {
                      const tileInfo =
                        currentSectionLayout[parseInt(sectionKeys[i], 10)];
                      if (tileInfo.school === newPowerInfo.school) {
                        for (let j = 0; j < tileInfo.powers.length; j++) {
                          const powerInfo = tileInfo.powers[j];
                          if (powerInfo === newPowerInfo.id) {
                            replaceLocation.tile = i;
                            replaceLocation.row = j;
                            found = true;
                            break;
                          }
                        }
                        if (found) {
                          break;
                        }
                      }
                    }
                    // if we do find another power with the 'new' id
                    if (found) {
                      // swap old power into 'new' power location!
                      if (oldPowerInfo) {
                        currentSectionLayout[replaceLocation.tile].powers[
                          replaceLocation.row
                        ] = oldPowerInfo.id;
                      } else {
                        // unset old power
                        currentSectionLayout[replaceLocation.tile].powers =
                          currentSectionLayout[replaceLocation.tile].powers.filter(
                            (val) => val !== newVal,
                          );
                      }
                    }
                    // set the new field!
                    currentSectionLayout[parseInt(tileIdx, 10)].powers[powerIdx] =
                      newPowerInfo.id;
                  } else {
                    // unset power
                    currentSectionLayout[parseInt(tileIdx, 10)].powers =
                      currentSectionLayout[parseInt(tileIdx, 10)].powers.filter(
                        (val) => val !== oldVal,
                      );
                  }
                } else {
                  // this is a change to discipline school
                  currentSectionLayout[parseInt(tileIdx, 10)] = {
                    school: newVal,
                    powers: [],
                    level: levels[newVal as keyof CharacterDisciplines] || 0,
                  };
                  // check to see if we need to add new blank tiles
                  let selectedCounter = 0;
                  const layoutKeys = Object.keys(currentSectionLayout);
                  for (let i = 0; i < layoutKeys.length; i++) {
                    const tileInfo =
                      currentSectionLayout[parseInt(layoutKeys[i], 10)];
                    // while we're doing this, update tile levels
                    if (tileInfo.school && tileInfo.school !== '')
                      selectedCounter += 1;
                  }
                  if (
                    selectedCounter % 3 === 0 &&
                    selectedCounter === layoutKeys.length
                  ) {
                    for (let i = 0; i < 3; i++) {
                      currentSectionLayout[layoutKeys.length + i] = {
                        school: '',
                        powers: [],
                        level: 0,
                      };
                    }
                  } else {
                    if (
                      selectedCounter - (selectedCounter % 3) <
                      layoutKeys.length - 3
                    ) {
                      // if no disciplines are selected in the final 3
                      // then we should remove them to save space
                      let slice = true;
                      for (let i = 0; i < 3; i++) {
                        const tile =
                          currentSectionLayout[layoutKeys.length - (i + 1)];
                        if (tile.school && tile.school !== '') {
                          slice = false;
                          break;
                        }
                      }
                      if (slice) {
                        for (let i = 0; i < 3; i++) {
                          delete currentSectionLayout[layoutKeys.length - (i + 1)];
                        }
                      }
                    }
                  }
                }
                // temporarily apply changes
                setSectionLayout({ ...currentSectionLayout });
                // pass this info higher up!
                if (onPowerChange) {
                  onPowerChange(oldVal, newVal, schoolChange);
                }
              }}
            />
          );
        })}
    </CharacterSheetSection>
  );
};
export default DisciplineSection;
