import React from 'react';

// types
import { CharacterBeliefs } from '../../../types/Character';

// local files
import CharacterSheetSection from './CharacterSheetSection';
import './BeliefsSection.scss';

interface BeliefsSectionProps {
  beliefs: CharacterBeliefs;
  onChange?: (field: string, val: string) => void;
}

interface BeliefsSectionTileProps {
  title: string;
  text: string;
  onChange?: (val: string) => void;
}

const BeliefsTile = ({ title, text, onChange }: BeliefsSectionTileProps) => {
  return (
    <div className="charactersheet-beliefs-tile">
      <span>
        <h3>{title}</h3>
      </span>
      <textarea
        rows={8}
        value={text}
        onChange={(event: React.ChangeEvent<HTMLTextAreaElement>) => {
          if (onChange) {
            onChange(event.target.value);
          }
        }}
      />
    </div>
  );
};

const BeliefsSection = ({ beliefs, onChange }: BeliefsSectionProps) => {
  return (
    <CharacterSheetSection className="charactersheet-beliefs-inner" title="Beliefs">
      <BeliefsTile
        title="Chronicle Tenets"
        text={beliefs.tenets}
        onChange={(val) => {
          if (onChange) {
            onChange('tenets', val);
          }
        }}
      />
      <BeliefsTile
        title="Convictions"
        text={beliefs.convictions}
        onChange={(val) => {
          if (onChange) {
            onChange('convictions', val);
          }
        }}
      />
      <BeliefsTile
        title="Touchstones"
        text={beliefs.touchstones}
        onChange={(val) => {
          if (onChange) {
            onChange('touchstones', val);
          }
        }}
      />
    </CharacterSheetSection>
  );
};
export default BeliefsSection;
