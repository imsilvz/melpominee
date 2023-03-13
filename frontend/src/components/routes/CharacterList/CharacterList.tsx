import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '../../../redux/hooks';
import { selectClans } from '../../../redux/reducers/masterdataReducer';

// types
import { CharacterHeader } from '../../../types/Character';

// local files
import './CharacterList.scss';

interface CharacterItemProps {
  character: CharacterHeader;
}

interface CharacterListResponse {
  success: boolean;
  error?: string;
  characterList?: CharacterHeader[];
}

const CharacterItem = ({ character }: CharacterItemProps) => {
  const navigate = useNavigate();
  const clanData = useAppSelector(selectClans);
  return (
    // eslint-disable-next-line jsx-a11y/click-events-have-key-events, jsx-a11y/no-static-element-interactions
    <div
      className="characterlist-listitem"
      onClick={() => navigate(`/character/${character.id}/`)}
    >
      <div className="characterlist-listitem-id">
        <span>#{character.id}</span>
      </div>
      <div className="characterlist-listitem">
        <h2>{character.name}</h2>
        <h3>{(clanData && clanData[character.clan]?.name) || 'Mortal'}</h3>
      </div>
    </div>
  );
};

const CharacterList = () => {
  const [characterList, setCharacterList] = useState<CharacterHeader[] | null>(
    null
  );
  const GetCharacterList = async () => {
    const listRequest = await fetch('/api/vtmv5/character/');
    if (listRequest.ok) {
      const listJson =
        await (listRequest.json() as Promise<CharacterListResponse>);
      if (listJson.success && listJson.characterList) {
        setCharacterList(listJson.characterList);
      }
    }
  };

  useEffect(() => {
    GetCharacterList().catch(console.error);
  }, []);

  return (
    <div className="characterlist-container">
      <div className="characterlist-panel">
        <div className="characterlist-header">
          <h1>Character List</h1>
        </div>
        <div className="characterlist-list">
          {characterList &&
            characterList.map((character) => (
              <CharacterItem
                key={`characterlist-character${character.id}`}
                character={character}
              />
            ))}
        </div>
      </div>
    </div>
  );
};
export default CharacterList;
