// a

export interface Character {
  id: number;
  name: string;
  concept: string;
  chronicle: string;
  ambition: string;
  desire: string;
  sire: string;
  generation: number;
  clan: string;
  predatorType: string;

  attributes: CharacterAttributes;
  skills: CharacterSkills;
  secondaryStats: CharacterSecondaryStats;

  disciplines: CharacterDisciplines;
  disciplinePowers: string[];

  hunger: number;
  resonance: string;
  bloodPotency: number;
}

export interface CharacterHeader {
  id: number;
  name: string;
  concept: string;
  chronicle: string;
  ambition: string;
  desire: string;
  sire: string;
  generation: number;
  clan: string;
  predatorType: string;
  hunger: number;
  resonance: string;
  bloodPotency: number;
}

export interface CharacterAttributes {
  strength: number;
  dexterity: number;
  stamina: number;
  charisma: number;
  manipulation: number;
  composure: number;
  intelligence: number;
  wits: number;
  resolve: number;
}

export interface CharacterSkill {
  speciality: string;
  score: number;
}

export interface CharacterSkills {
  athletics: CharacterSkill;
  brawl: CharacterSkill;
  craft: CharacterSkill;
  drive: CharacterSkill;
  firearms: CharacterSkill;
  melee: CharacterSkill;
  larceny: CharacterSkill;
  stealth: CharacterSkill;
  survival: CharacterSkill;
  animalKen: CharacterSkill;
  ettiquette: CharacterSkill;
  insight: CharacterSkill;
  intimidation: CharacterSkill;
  leadership: CharacterSkill;
  performance: CharacterSkill;
  persuasion: CharacterSkill;
  streetwise: CharacterSkill;
  subterfuge: CharacterSkill;
  academics: CharacterSkill;
  awareness: CharacterSkill;
  finance: CharacterSkill;
  investigation: CharacterSkill;
  medicine: CharacterSkill;
  occult: CharacterSkill;
  politics: CharacterSkill;
  science: CharacterSkill;
  technology: CharacterSkill;
}

export interface CharacterStat {
  baseValue: number;
  superficialDamage: number;
  aggravatedDamage: number;
}

export interface CharacterSecondaryStats {
  health: CharacterStat;
  willpower: CharacterStat;
  humanity: CharacterStat;
}

export interface CharacterDisciplines {
  Animalism?: number;
  Auspex?: number;
  BloodSorcery?: number;
  Celerity?: number;
  Dominate?: number;
  Fortitude?: number;
  Obfuscate?: number;
  Oblivion?: number;
  Potence?: number;
  Presence?: number;
  Protean?: number;
  ThinBloodAlchemy?: number;
}
