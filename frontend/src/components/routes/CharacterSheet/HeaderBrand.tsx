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
  (async () => {
    const clanImage = (
      await (import(
        `../../../assets/clans/${clan}.svg`
      ) as Promise<ImportedItem>)
    ).default;
    ClanImages.set(clan, clanImage);
    // eslint-disable-next-line no-console
  })().catch(console.error);
});

const HeaderBrand = ({ clan }: HeaderBrandProps) => {
  let clanImage = ClanImages.get('generic');
  if (clan && ClanImages.has(clan)) {
    clanImage = ClanImages.get(clan) as string;
  }
  return (
    <div className="charactersheet-header-brand">
      <div className="charactersheet-header-brand-container">
        <img src={clanImage} alt="" />
      </div>
    </div>
  );
};
export default HeaderBrand;
