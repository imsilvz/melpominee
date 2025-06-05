import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '../../../redux/hooks';
import { selectClans } from '../../../redux/reducers/masterdataReducer';
import { selectUserRole } from '../../../redux/reducers/userReducer';

// types
import { CharacterHeader } from '../../../types/Character';

// local files
import './CharacterList.scss';
import LoadingSpinner from '../../shared/LoadingSpinner/LoadingSpinner';
import ToggleSwitch from '../../shared/ToggleSwitch/ToggleSwitch';

interface ExtendedCharacterHeader extends CharacterHeader {
  owner: string;
}

interface CharacterItemProps {
  character: ExtendedCharacterHeader;
}

interface CharacterListResponse {
  success: boolean;
  error?: string;
  characterList?: ExtendedCharacterHeader[];
}

interface CharacterCreateResponse {
  success: boolean;
  error?: string;
  characterId?: number;
}

const CharacterItem = ({ character }: CharacterItemProps) => {
  const navigate = useNavigate();
  const clanData = useAppSelector(selectClans);
  const userRole = useAppSelector(selectUserRole);
  return (
    // eslint-disable-next-line jsx-a11y/click-events-have-key-events, jsx-a11y/no-static-element-interactions
    <div
      className="characterlist-listitem"
      onClick={() => navigate(`/character/${character.id}/`)}
    >
      <div className="characterlist-listitem-id">
        <span>#{character.id}</span>
      </div>
      <div className="characterlist-listitem-info">
        <h2>{character.name}</h2>
        <h3>{(clanData && clanData[character.clan]?.name) || 'Mortal'}</h3>
      </div>
      <div className="characterlist-listitem-game">
        <h6>{character.owner}</h6>
        <h5>{character.chronicle}</h5>
      </div>
    </div>
  );
};

const CharacterList = () => {
  const navigate = useNavigate();
  const userRole = useAppSelector(selectUserRole);
  const [loading, setLoading] = useState<boolean>(true);
  const [adminMode, setAdminMode] = useState<boolean>(false);
  const [characterList, setCharacterList] = useState<ExtendedCharacterHeader[] | null>(null);
  const GetCharacterList = async (adminMode: boolean) => {
    let url = '/api/vtmv5/character/';
    if (adminMode) { url = `${url}?adminView=true` }
    const listRequest = await fetch(url);
    if (listRequest.ok) {
      const listJson = await (listRequest.json() as Promise<CharacterListResponse>);
      if (listJson.success && listJson.characterList) {
        listJson.characterList.sort((a, b) => {
          if (a.id > b.id) {
            return 1;
          } else if (a.id < b.id) {
            return -1;
          }
          return 0;
        })
        setCharacterList(listJson.characterList);
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    GetCharacterList(adminMode).catch(console.error);
  }, [adminMode]);

  return (
    <div className="characterlist-container">
      {loading ? (
        <LoadingSpinner />
      ) : (
        <div className="characterlist-panel">
          <div className="characterlist-header">
            <h1>Character List</h1>
            {userRole === 'admin' && <ToggleSwitch label="View All" checked={adminMode} onSwitch={setAdminMode} />}
          </div>
          <div className="characterlist-list">
            {characterList &&
              characterList.map((character) => (
                <CharacterItem
                  key={`characterlist-character${character.id}`}
                  character={character}
                />
              ))}
            {/* eslint-disable-next-line jsx-a11y/click-events-have-key-events, jsx-a11y/no-static-element-interactions */}
            <div
              className="characterlist-additem"
              onClick={() => {
                (async () => {
                  const createRequest = await fetch(`/api/vtmv5/character/`, {
                    method: 'POST',
                    headers: {
                      Accept: 'application/json',
                    },
                  });
                  if (createRequest.ok) {
                    const createJson: CharacterCreateResponse =
                      await (createRequest.json() as Promise<CharacterCreateResponse>);
                    if (createJson.characterId) {
                      navigate(`/character/${createJson.characterId}/`);
                    }
                  }
                })().catch(console.error);
              }}
            >
              <span>Create New Character</span>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
export default CharacterList;
