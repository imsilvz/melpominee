import React, { ReactNode, useState } from "react";

// redux
import { useAppSelector } from "../../../redux/hooks";
import { selectDisciplinePowers, selectPredatorTypes } from "../../../redux/reducers/masterdataReducer";
import { selectTooltipId, selectTooltipType } from "../../../redux/reducers/tooltipReducer";

// local imports
import './TooltipProvider.scss';

// types
import { DisciplinePower } from "../../../types/Discipline";
import { PredatorType } from '../../../types/PredatorType';

const DisciplinePowerTooltip = ({ data }: { data: DisciplinePower }) => {
  return (
    <div className="discipline-power-tooltip-inner">
      <h3>{data.name}</h3>
      <h4>{data.school} {'●'.repeat(data.level)}{((data.amalgam?.level || 0) > 0) && (<> (Amalgam {data.amalgam?.school} {'●'.repeat(data.amalgam!.level as number)})</>)}</h4>
      <p>{data.effect}</p>
      <h5>Additional Notes</h5>
      <p>{data.additionalNotes}</p>
      <p><b>Cost:</b> {data.cost}{data.dicePool !== 'N/A' && (<> | <b>Test:</b> {data.dicePool}{data.opposingPool !== 'N/A' && ` vs ${data.opposingPool}`}</>)}</p>
      {data.source && (<p><b>Source:</b> {data.source}</p>)}
    </div>
  )
}

const PredatorTypeTooltip = ({ data }: { data: PredatorType }) => {
  return (
    <div className="predator-tooltip-inner">
      <h3>{data.name}</h3>
      <p>{data.description}</p>
      <ul>
        {data.effectList.map((item) => (
          <li>{item}</li>
        ))}
      </ul>
    </div>
  )
}

const TooltipProvider = ({ children }: { children: ReactNode }) => {
  const tooltipType = useAppSelector(selectTooltipType);
  const tooltipId = useAppSelector(selectTooltipId);
  const disciplinePowers = useAppSelector(selectDisciplinePowers);
  const predatorTypes = useAppSelector(selectPredatorTypes);

  let tooltipData = null;
  if (tooltipId) {
    if (tooltipType === 'predator_type') {
      console.log(predatorTypes);
      if (predatorTypes.hasOwnProperty(tooltipId)) {
        tooltipData = predatorTypes[tooltipId];
      }
    } else if (tooltipType === 'discipline_power') {
      if (disciplinePowers.hasOwnProperty(tooltipId)) {
        tooltipData = disciplinePowers[tooltipId];
      }
    }
  }

  return (
    <>
      {children}
      {tooltipData !== null && (
        <div className="melpominee-tooltip">
          {tooltipType === 'discipline_power' && (
            <DisciplinePowerTooltip data={tooltipData as DisciplinePower} />
          )}
          {tooltipType === 'predator_type' && (
            <PredatorTypeTooltip data={tooltipData as PredatorType} />
          )}
        </div>
      )}
    </>
  )
}
export default TooltipProvider;