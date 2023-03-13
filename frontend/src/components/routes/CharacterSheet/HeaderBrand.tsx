import React, { useEffect, useState } from 'react';
import './HeaderBrand.scss';

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

const HeaderBrand = ({ clan }: HeaderBrandProps) => {
  const [logoMap, setLogoMap] = useState<Map<string, string> | null>(null);
  const [imageMap, setImageMap] = useState<Map<string, string> | null>(null);

  useEffect(() => {
    // cache to prevent reloads
    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-explicit-any
    (window as any).headerPreload = {
      logos: [],
      images: [],
    };

    // preload images on mount
    const ClanLogos = new Map<string, string>();
    const ClanImages = new Map<string, string>();
    ClanList.forEach((clanId) => {
      const clanImage = new Image();
      ClanImages.set(clanId, `/images/clan/${clanId}.svg`);
      clanImage.src = `/images/clan/${clanId}.svg`;
      // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-unsafe-call, @typescript-eslint/no-explicit-any
      (window as any).headerPreload.images.push(clanImage);
      if (clanId !== 'generic') {
        const clanLogo = new Image();
        clanLogo.src = `/images/clan/symbol/${clanId}.png`;
        ClanLogos.set(clanId, `/images/clan/symbol/${clanId}.png`);
        // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-unsafe-call, @typescript-eslint/no-explicit-any
        (window as any).headerPreload.logos.push(clanLogo);
      }
    });
    setLogoMap(ClanLogos);
    setImageMap(ClanImages);
  }, []);

  let clanLogo = '';
  let clanImage = imageMap?.get('generic');
  if (clan && imageMap?.has(clan)) {
    clanImage = imageMap?.get(clan) as string;
    if (logoMap?.has(clan)) {
      clanLogo = logoMap?.get(clan) as string;
    }
  }
  return (
    <div className={`charactersheet-header-brand ${clan}`}>
      {clanLogo && clanLogo !== '' && <img loading="lazy" src={clanLogo} alt="" />}
      <div className={`charactersheet-header-brand-container ${clan}`}>
        <img loading="lazy" src={clanImage} alt="" />
      </div>
    </div>
  );
};
export default HeaderBrand;
