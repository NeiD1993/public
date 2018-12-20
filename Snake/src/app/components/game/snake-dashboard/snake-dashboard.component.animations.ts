import { propertyAnimation, rotate3dAngleToString } from "../../animations";
import { style } from "@angular/animations";

export const blueColorStyle = style({ color: 'blue' });

export const greenColorStyle = style({ color: 'green' });

export const redColorStyle = style({ color: 'red' });

export const synchronousRotation3dAnimation = (duration: number, colors: { [key: string]: string }) => {
    return propertyAnimation(duration, [
        { color: colors['first'], transform: rotate3dAngleToString(0) },
        { color: colors['third'], transform: rotate3dAngleToString(90) },
        { color: colors['second'], transform: rotate3dAngleToString(180) },
        { color: colors['first'], transform: rotate3dAngleToString(270) },
        { color: colors['third'], transform: rotate3dAngleToString(360) }
    ]);
};