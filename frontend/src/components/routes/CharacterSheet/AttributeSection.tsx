import React from 'react';

// types
import { CharacterAttributes } from '../../../types/Character';

// local files
import { toTitleCase } from '../../../util/character';
import StatDots from './StatDots';
import './AttributeSection.scss';

const AttributeSection = ({
  attributes,
}: {
  attributes: CharacterAttributes;
}) => {
  const physicalAttributes = ['strength', 'dexterity', 'stamina'];
  const socialAttributes = ['charisma', 'manipulation', 'composure'];
  const mentalAttributes = ['intelligence', 'wits', 'resolve'];
  return (
    <div className="charactersheet-attributes">
      <div className="charactersheet-attributes-header">
        <div className="charactersheet-attributes-header-title">
          <h2>Attributes</h2>
        </div>
        <div className="charactersheet-attributes-header-divider" />
      </div>
      <div className="charactersheet-attributes-inner">
        {/* Physical Attributes */}
        <div className="charactersheet-attributes-section">
          <div className="charactersheet-attributes-item">
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
          <div className="charactersheet-attributes-item">
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
          <div className="charactersheet-attributes-item">
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
                />
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
export default AttributeSection;
