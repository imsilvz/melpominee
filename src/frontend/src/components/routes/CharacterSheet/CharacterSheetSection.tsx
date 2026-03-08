import React, { PropsWithChildren } from 'react';

// local files
import './CharacterSheetSection.scss';

interface CharacterSheetSectionProps {
  className: string;
  title: string;
}

const CharacterSheetSection = ({
  children,
  className,
  title,
}: PropsWithChildren<CharacterSheetSectionProps>) => {
  return (
    <div className="charactersheet-section">
      <div className="charactersheet-section-header">
        <div className="charactersheet-section-header-divider" />
        <div className="charactersheet-section-header-title">
          <h2>{title}</h2>
        </div>
        <div className="charactersheet-section-header-divider" />
      </div>
      <div className={className}>{children}</div>
    </div>
  );
};
export default CharacterSheetSection;
