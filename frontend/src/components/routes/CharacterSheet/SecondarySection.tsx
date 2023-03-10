import React from 'react';

// types
import { Character } from '../../../types/Character';

// local files
import HealthTracker from './HealthTracker';
import './SecondarySection.scss';

const SecondarySection = ({ character }: { character: Character }) => {
  const maxHealth = character.attributes.stamina + 3;
  const maxWillpower =
    character.attributes.composure + character.attributes.resolve;
  const humanityValue = character.secondaryStats.humanity.baseValue;
  return (
    <div className="charactersheet-secondary">
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Health</h4>
          <HealthTracker rootKey="secondarystat-health" value={maxHealth} />
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Willpower</h4>
          <HealthTracker
            rootKey="secondarystat-willpower"
            value={maxWillpower}
          />
        </div>
        <div className="charactersheet-secondary-healthrow-item">
          <h4>Humanity</h4>
          <HealthTracker
            rootKey="secondarystat-humanity"
            value={humanityValue}
          />
        </div>
      </div>
      <div className="charactersheet-secondary-col">
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Resonance</h4>
          <span style={{ fontSize: '0.875rem' }}>{character.resonance}</span>
        </div>
        <div className="charactersheet-secondary-hungerrow-item">
          <h4>Hunger</h4>
          <HealthTracker
            rootKey="secondarystat-hunger"
            dotCount={5}
            value={character.hunger}
          />
        </div>
      </div>
    </div>
  );
};
export default SecondarySection;
