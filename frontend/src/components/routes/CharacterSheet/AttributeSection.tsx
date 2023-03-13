import React from 'react';

// types
import { CharacterAttributes } from '../../../types/Character';

// local files
import { toTitleCase } from '../../../util/character';
import StatDots from './StatDots';
import './AttributeSection.scss';
import CharacterSheetSection from './CharacterSheetSection';

interface AttributeSectionProps {
  attributes: CharacterAttributes;
  onChange?: (attribute: string, value: number) => void;
}

const AttributeSection = ({ attributes, onChange }: AttributeSectionProps) => {
  const physicalAttributes = ['strength', 'dexterity', 'stamina'];
  const socialAttributes = ['charisma', 'manipulation', 'composure'];
  const mentalAttributes = ['intelligence', 'wits', 'resolve'];
  return (
    <CharacterSheetSection
      className="charactersheet-attributes-inner"
      title="Attributes"
    >
      {/* Physical Attributes */}
      <div className="charactersheet-attributes-section">
        <div className="charactersheet-attributes-subheader">
          <h3>Physical</h3>
        </div>
        {physicalAttributes.map((attr) => (
          <div
            key={`attributes_physical_${attr}`}
            className="charactersheet-attribute-item"
          >
            <div className="charactersheet-attribute-info">
              <span>{toTitleCase(attr)}</span>
            </div>
            <div className="charactersheet-attribute-score">
              <StatDots
                rootKey={`attributes_physical_${attr}`}
                value={attributes[attr as keyof CharacterAttributes]}
                onChange={(oldVal, newVal) => {
                  if (onChange) {
                    onChange(attr, newVal);
                  }
                }}
              />
            </div>
          </div>
        ))}
      </div>
      <div className="charactersheet-attributes-divider">
        <div className="charactersheet-attributes-divider-inner" />
      </div>
      {/* Social Attributes */}
      <div className="charactersheet-attributes-section">
        <div className="charactersheet-attributes-subheader">
          <h3>Social</h3>
        </div>
        {socialAttributes.map((attr) => (
          <div
            key={`attributes_social_${attr}`}
            className="charactersheet-attribute-item"
          >
            <div className="charactersheet-attribute-info">
              <span>{toTitleCase(attr)}</span>
            </div>
            <div className="charactersheet-attribute-score">
              <StatDots
                rootKey={`attributes_social_${attr}`}
                value={attributes[attr as keyof CharacterAttributes]}
                onChange={(oldVal, newVal) => {
                  if (onChange) {
                    onChange(attr, newVal);
                  }
                }}
              />
            </div>
          </div>
        ))}
      </div>
      <div className="charactersheet-attributes-divider">
        <div className="charactersheet-attributes-divider-inner" />
      </div>
      {/* Mental Attributes */}
      <div className="charactersheet-attributes-section">
        <div className="charactersheet-attributes-subheader">
          <h3>Mental</h3>
        </div>
        {mentalAttributes.map((attr) => (
          <div
            key={`attributes_mental_${attr}`}
            className="charactersheet-attribute-item"
          >
            <div className="charactersheet-attribute-info">
              <span>{toTitleCase(attr)}</span>
            </div>
            <div className="charactersheet-attribute-score">
              <StatDots
                rootKey={`attributes_mental_${attr}`}
                value={attributes[attr as keyof CharacterAttributes]}
                onChange={(oldVal, newVal) => {
                  if (onChange) {
                    onChange(attr, newVal);
                  }
                }}
              />
            </div>
          </div>
        ))}
      </div>
    </CharacterSheetSection>
  );
};
export default AttributeSection;
