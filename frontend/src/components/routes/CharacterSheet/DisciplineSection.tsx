import React, { useEffect, useState } from 'react';

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
  school: string;
}

interface DisciplineTileProps {
  id: string;
  level: number;
  school: string;
}

const PowerRow = ({ id, school }: PowerRowProps) => {
  const disciplines = useAppSelector(selectDisciplines);
  return (
    <div className="charactersheet-disciplines-power">
      <select
        value={school || undefined}
        disabled={!Object.prototype.hasOwnProperty.call(disciplines, school)}
      >
        {/* eslint-disable-next-line jsx-a11y/control-has-associated-label */}
        <option value="" />
        {Object.prototype.hasOwnProperty.call(disciplines, school) &&
          disciplines[school].powers.map((power, powerIdx) => (
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
  console.log(disciplines[school]);
  return (
    <div className="charactersheet-disciplines-block">
      <div className="charactersheet-disciplines-school">
        <div className="charactersheet-disciplines-school-select">
          <select value={school || undefined}>
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
          school={school}
        />
      ))}
    </div>
  );
};

const DisciplineSection = ({
  levels,
  powers,
}: {
  levels: CharacterDisciplines;
  powers: string[];
}) => {
  const disciplinePowers = useAppSelector(selectDisciplinePowers);

  // find character disciplines
  const charDisciplinesList = powers
    .reduce((prev: string[], curr) => {
      if (Object.prototype.hasOwnProperty.call(disciplinePowers, curr)) {
        const power = disciplinePowers[curr];
        if (!prev.includes(power.school)) {
          prev.push(power.school);
        }
      }
      return prev;
    }, [])
    .sort();

  return (
    <div className="charactersheet-disciplines">
      <div className="charactersheet-disciplines-header">
        <div className="charactersheet-disciplines-header-title">
          <h2>Disciplines</h2>
        </div>
        <div className="charactersheet-disciplines-header-divider" />
      </div>
      <div className="charactersheet-disciplines-inner">
        {Array.from(Array(6), (_block, i) => i).map((_, idx) => {
          let schoolLevel = 0;
          let chosenSchool = '';
          if (charDisciplinesList.length > idx) {
            chosenSchool = charDisciplinesList[idx];
            if (Object.prototype.hasOwnProperty.call(levels, chosenSchool)) {
              schoolLevel = levels[
                chosenSchool as keyof CharacterDisciplines
              ] as number;
            }
          }
          return (
            <DisciplineTile
              id={`disciplines-block${idx}`}
              key={`disciplines-block${idx}`}
              level={schoolLevel}
              school={chosenSchool}
            />
          );
        })}
      </div>
    </div>
  );
};
export default DisciplineSection;
