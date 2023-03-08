import React from 'react';

interface ImportedItem {
  default: string;
}

interface HeaderBrandProps {
  clan: string;
}

const ClanList = [
  'generic',
  'banu_haqim',
  'brujah',
  'gangrel',
  'hecata',
  'lasombra',
  'malkavian',
  'nosferatu',
  'ravnos',
  'salubri',
  'ministry',
  'toreador',
  'tremere',
  'tzimisce',
  'ventrue',
];
const ClanImages = new Map<string, string>();
ClanList.forEach((clan) => {
  ClanImages.set(clan, `/images/clan/${clan}.svg`);
});

const HeaderBrand = ({ clan }: HeaderBrandProps) => {
  let clanImage = ClanImages.get('generic');
  if (clan && ClanImages.has(clan)) {
    clanImage = ClanImages.get(clan) as string;
  }
  return (
    <div className="charactersheet-header-brand">
      <div className="charactersheet-header-brand-container">
        <img loading="lazy" src={clanImage} alt="" />
      </div>
    </div>
  );
};
export default HeaderBrand;
