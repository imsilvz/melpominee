import React, { useEffect, useRef, useState } from 'react';

// redux
import { useAppSelector } from '../../../redux/hooks';
import {
  selectDisciplinePowers,
  selectDisciplines,
} from '../../../redux/reducers/masterdataReducer';

// types
import { CharacterDisciplines } from '../../../types/Character';

// local files
import StatDots from './StatDots';
import './DisciplineSection.scss';

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
    schoolChange?: boolean
  ) => void;
}

interface DisciplineSectionProps {
  characterId: number;
  levels: CharacterDisciplines;
  powers: string[];
  onLevelChange?: (school: string, oldVal: number, newVal: number) => void;
  onPowerChange?: (
    oldVal: string,
    newVal: string,
    schoolChange?: boolean
  ) => void;
}

const PowerRow = ({ id, level, school, power, onChange }: PowerRowProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  return (
    <div className="charactersheet-disciplines-power">
      <select
        value={power || ''}
        disabled={
          !Object.prototype.hasOwnProperty.call(disciplines, school) ||
          level === 0
        }
        onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
          if (onChange) {
            onChange(power, event.target.value);
          }
        }}
      >
        {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
        <option value="" />
        {Object.prototype.hasOwnProperty.call(disciplines, school) &&
          disciplines[school].powers
            .filter((val) => val.level <= level)
            .map((opt) => (
              <option key={`${id}-power-option-${opt.id}`} value={opt.id}>
                {opt.name}
              </option>
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
  const [sectionLayout, setSectionLayout] =
    useState<DisciplineSectionLayout | null>(null);

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
      // assuming we have 5 rows, figure out how many tiles we need for each discipline
      // then sort them in alphabetical order
      const discTileList = Array.from(discCountMap.entries())
        .reduce((output: string[], kvp) => {
          for (let i = 0; i < Math.ceil(kvp[1] / 5); i++) {
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
      // first we must iterate all tiles and their assigned powers
      // if any powers are no longer in our power list, remove them
      // if any powers are not currently assigned in our layout, set them
      const layoutPowers: string[] = [];
      const layoutKeys = Object.keys(newSectionLayout);
      for (let i = 0; i < layoutKeys.length; i++) {
        const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
        layoutPowers.push(...tileInfo.powers);
      }
      const toAdd = powers.filter((powerId) => !layoutPowers.includes(powerId));
      const toRemove = layoutPowers.filter(
        (powerId) => powerId && !powers.includes(powerId)
      );
      // first, remove powers that do not belong
      toRemove.forEach((removeVal) => {
        for (let i = 0; i < layoutKeys.length; i++) {
          const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
          tileInfo.powers = tileInfo.powers.filter(
            (powerId) => powerId !== removeVal
          );
        }
      });
      // next, add powers that are missing
      toAdd.forEach((addVal) => {
        let added = false;
        const powerInfo = disciplinePowers[addVal];
        for (let i = 0; i < layoutKeys.length; i++) {
          const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
          if (tileInfo.school === powerInfo.school) {
            // ensure we do not insert into a tile where there is no room!
            if (tileInfo.powers.length < 5) {
              // add power
              // then sort by level, and then alphabetically
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
              added = true;
              break;
            }
          }
        }
        // check if we were successful
        if (!added) {
          // this means we were unable to find a tile with open space
          // so we should now search tiles with empty schools
          for (let i = 0; i < layoutKeys.length; i++) {
            const tileInfo = newSectionLayout[parseInt(layoutKeys[i], 10)];
            if (tileInfo.school === '') {
              // this tile is empty, so we can set it up
              tileInfo.school = powerInfo.school;
              tileInfo.powers = [addVal];
              tileInfo.level =
                levels[powerInfo.school as keyof CharacterDisciplines] || 0;
              added = true;
              break;
            }
          }
          if (!added) {
            // this means there are no valid empty rows
            // so we must add a trio of new rows
            // and set the first of this new trio to accomodate
            const offset = Object.keys(newSectionLayout).length;
            newSectionLayout[offset] = {
              school: powerInfo.school,
              powers: [addVal],
              level:
                levels[powerInfo.school as keyof CharacterDisciplines] || 0,
            };
            for (let i = 1; i < 3; i++) {
              newSectionLayout[offset + i] = {
                school: '',
                powers: [],
                level: 0,
              };
            }
          }
        }
      });
      sectionLayoutRef.current = newSectionLayout;
    }
    setSectionLayout(sectionLayoutRef.current);
  }, [disciplinePowers, characterId, levels, powers]);

  return (
    <div className="charactersheet-disciplines">
      <div className="charactersheet-disciplines-header">
        <div className="charactersheet-disciplines-header-divider" />
        <div className="charactersheet-disciplines-header-title">
          <h2>Disciplines</h2>
        </div>
        <div className="charactersheet-disciplines-header-divider" />
      </div>
      <div className="charactersheet-disciplines-inner">
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
                  // get tile info and set new level
                  const tileInfo = currentSectionLayout[parseInt(tileIdx, 10)];
                  tileInfo.level = newVal;
                  // iterate all tiles and remove any disciplines
                  // which do not meet level or amalgam requirements
                  setSectionLayout({ ...currentSectionLayout });
                  if (onLevelChange) {
                    onLevelChange(tileInfo.school, oldVal, newVal);
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
                            currentSectionLayout[
                              replaceLocation.tile
                            ].powers.filter((val) => val !== newVal);
                        }
                      }
                      // set the new field!
                      currentSectionLayout[parseInt(tileIdx, 10)].powers[
                        powerIdx
                      ] = newPowerInfo.id;
                    } else {
                      // unset power
                      currentSectionLayout[parseInt(tileIdx, 10)].powers =
                        currentSectionLayout[
                          parseInt(tileIdx, 10)
                        ].powers.filter((val) => val !== oldVal);
                    }
                  } else {
                    // this is a change to discipline school
                    currentSectionLayout[parseInt(tileIdx, 10)] = {
                      school: newVal,
                      powers: [],
                      level: levels[newVal as keyof CharacterDisciplines] || 0,
                    };
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
      </div>
    </div>
  );
};
export default DisciplineSection;
