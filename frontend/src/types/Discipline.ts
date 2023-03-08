export interface DisciplineAmalgam {
  level?: number;
  school: string;
}

export interface DisciplinePower {
  id: string;
  name: string;
  school: string;
  level: number;
  prerequisite?: string;
  amalgam?: DisciplineAmalgam;
  cost: string;
  duration: string;
  dicePool: string;
  opposingPool: string;
  effect: string;
  additionalNotes: string;
  source: string;
}

export interface Discipline {
  id: string;
  name: string;
  powers: DisciplinePower[];
}
