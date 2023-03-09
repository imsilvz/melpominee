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
}

interface DisciplineTileProps {
  id: string;
  level: number;
  school: string;
}

interface DisciplineSectionProps {
  characterId: number;
  levels: CharacterDisciplines;
  powers: string[];
}

const PowerRow = ({ id, level, school }: PowerRowProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  return (
    <div className="charactersheet-disciplines-power">
      <select
        value={school || undefined}
        disabled={!Object.prototype.hasOwnProperty.call(disciplines, school)}
        onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {}}
      >
        {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
        <option value="" />
        {Object.prototype.hasOwnProperty.call(disciplines, school) &&
          disciplines[school].powers
            .filter((val) => val.level <= level)
            .map((power, powerIdx) => (
              <option key={`${id}-power-option${powerIdx}`} value={power.id}>
                {power.name}
              </option>
            ))}
      </select>
      <span />
    </div>
  );
};

const DisciplineTile = ({ id, level, school }: DisciplineTileProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  const disciplineList = Array.from(Object.keys(disciplines)).sort();
  return (
    <div className="charactersheet-disciplines-block">
      <div className="charactersheet-disciplines-school">
        <div className="charactersheet-disciplines-school-select">
          <select
            value={school || undefined}
            onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {}}
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
        <StatDots rootKey={`${id}-dots`} value={level} />
      </div>
      {Array.from(Array(5), (_skip, i) => i).map((_skipRow, rowIdx) => (
        <PowerRow
          id={`${id}-power${rowIdx}`}
          key={`${id}-power${rowIdx}`}
          level={level}
          school={school}
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
      console.log('UPDATE');
    }
    setSectionLayout(sectionLayoutRef.current);
  }, [disciplinePowers, characterId, levels, powers]);

  return (
    <div className="charactersheet-disciplines">
      <div className="charactersheet-disciplines-header">
        <div className="charactersheet-disciplines-header-title">
          <h2>Disciplines</h2>
        </div>
        <div className="charactersheet-disciplines-header-divider" />
      </div>
      <div className="charactersheet-disciplines-inner">
        {sectionLayout &&
          Object.keys(sectionLayout).map((idx) => {
            const tileLayout: DisciplineSectionTile =
              sectionLayout[parseInt(idx, 10)];
            return (
              <DisciplineTile
                id={`disciplines-tile${idx}`}
                key={`disciplines-tile${idx}`}
                level={tileLayout.level}
                school={tileLayout.school}
              />
            );
          })}
      </div>
    </div>
  );
};
export default DisciplineSection;
