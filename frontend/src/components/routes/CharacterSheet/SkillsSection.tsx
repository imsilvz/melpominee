import React, { useEffect, useState } from 'react';

// types
import { CharacterSkill, CharacterSkills } from '../../../types/Character';

// local files
import { toTitleCase } from '../../../util/character';
import StatDots from './StatDots';
import './SkillsSection.scss';

interface SkillsSectionProps {
  skills: CharacterSkills;
  onChange?: (skill: string, skillInfo: CharacterSkill) => void;
}

const CharacterSkillsList = [
  'athletics',
  'brawl',
  'craft',
  'drive',
  'firearms',
  'melee',
  'larceny',
  'stealth',
  'survival',
  'animalKen',
  'ettiquette',
  'insight',
  'intimidation',
  'leadership',
  'performance',
  'persuasion',
  'streetwise',
  'subterfuge',
  'academics',
  'awareness',
  'finance',
  'investigation',
  'medicine',
  'occult',
  'politics',
  'science',
  'technology',
];
const SkillsSection = ({ skills, onChange }: SkillsSectionProps) => {
  // break into columns
  let skillColumnIndex = -1;
  const skillColumns: string[][] = [];
  for (let i = 0; i < CharacterSkillsList.length; i++) {
    if (i % (CharacterSkillsList.length / 3) === 0) {
      skillColumnIndex += 1;
      skillColumns.push([]);
    }
    skillColumns[skillColumnIndex].push(CharacterSkillsList[i]);
  }

  return (
    <div className="charactersheet-skills">
      <div className="charactersheet-skills-header">
        <div className="charactersheet-skills-header-divider" />
        <div className="charactersheet-skills-header-title">
          <h2>Skills</h2>
        </div>
        <div className="charactersheet-skills-header-divider" />
      </div>
      <div className="charactersheet-skills-inner">
        {skillColumns.map((column, columnIdx) => (
          <div
            // eslint-disable-next-line react/no-array-index-key
            key={`skills_column${columnIdx}`}
            className="charactersheet-skills-section"
          >
            {column.map((skill) => (
              <div
                key={`skills_item_${skill}`}
                className="charactersheet-skill-item"
              >
                <div className="charactersheet-skills-item-info">
                  <span>{toTitleCase(skill)}</span>
                </div>
                <div className="charactersheet-skills-speciality">
                  <input
                    type="text"
                    value={skills[skill as keyof CharacterSkills].speciality}
                    onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                      if (onChange) {
                        const newSkillData = {
                          ...skills[skill as keyof CharacterSkills],
                        };
                        newSkillData.speciality = event.target.value;
                        onChange(skill, newSkillData);
                      }
                    }}
                  />
                </div>
                <div className="charactersheet-skills-item-score">
                  <StatDots
                    rootKey={`skills_item_${skill}_dots`}
                    value={skills[skill as keyof CharacterSkills].score}
                    onChange={(oldVal, newVal) => {
                      if (onChange) {
                        const newSkillData = {
                          ...skills[skill as keyof CharacterSkills],
                        };
                        newSkillData.score = newVal;
                        onChange(skill, newSkillData);
                      }
                    }}
                  />
                </div>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
};
export default SkillsSection;
