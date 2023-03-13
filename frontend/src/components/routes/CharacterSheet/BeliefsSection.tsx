import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './BeliefsSection.scss';

interface BeliefsSectionProps {
  id?: string;
  title?: string;
}

interface BeliefsSectionTileProps {
  id: string;
  title: string;
}

const BeliefsTile = ({ id, title }: BeliefsSectionTileProps) => {
  return (
    <div className="charactersheet-beliefs-tile">
      <span>
        <h3>{title}</h3>
      </span>
      <textarea rows={8} />
    </div>
  );
};

const BeliefsSection = ({ id, title }: BeliefsSectionProps) => {
  return (
    <CharacterSheetSection className="charactersheet-beliefs-inner" title="Beliefs">
      <BeliefsTile id="chronicleTenets" title="Chronicle Tenets" />
      <BeliefsTile id="convictions" title="Convictions" />
      <BeliefsTile id="Touchstones" title="Touchstones" />
    </CharacterSheetSection>
  );
};
export default BeliefsSection;
