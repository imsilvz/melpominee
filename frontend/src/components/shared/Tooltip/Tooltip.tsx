import React, { ReactElement, ReactNode, useEffect, useRef } from "react";

// local imports
import { useAppDispatch } from "../../../redux/hooks";
import { setTooltipData } from "../../../redux/reducers/tooltipReducer";

const Tooltip = ({ type, typeId, element }: { type: string, typeId: string, element: ReactElement}) => {
  const ref = useRef(null);
  const dispatch = useAppDispatch();

  const handleMouseEnter = (event: MouseEvent) => {
    console.log(type, typeId);
    dispatch(setTooltipData({
      tooltipType: type,
      tooltipId: typeId
    }));
  }
  
  const handleMouseLeave = (event: MouseEvent) => {
    dispatch(setTooltipData({
      tooltipType: '',
      tooltipId: ''
    }));
  }

  useEffect(() => {
    if (ref.current) {
      (ref.current as HTMLElement).addEventListener("mouseenter", handleMouseEnter);
      (ref.current as HTMLElement).addEventListener("mouseleave", handleMouseLeave);
    }
    return () => {
      if (ref.current) {
        (ref.current as HTMLElement).removeEventListener("mouseenter", handleMouseEnter);
        (ref.current as HTMLElement).removeEventListener("mouseleave", handleMouseLeave);
      }
    }
  }, [ref.current, type, typeId]);

  const childElement = React.cloneElement(element, {ref})
  return <>
    {childElement}
  </>
}
export default Tooltip;