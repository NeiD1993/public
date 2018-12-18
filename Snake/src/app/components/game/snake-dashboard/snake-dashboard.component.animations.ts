import { propertyAnimation } from "../game-animations";
import { style } from "@angular/animations";

export const blueColorStyle = style({ color: 'blue' });

export const greenColorStyle = style({ color: 'green' });

export const fullRotation3dAnimation = (duration: number, isDirect: boolean) =>
    propertyAnimation(duration, [
        { transform: rotate3dAngleToString(isDirect ? 0 : 360) },
        { transform: rotate3dAngleToString(isDirect ? 360 : 0) }
    ]);

const rotate3dAngleToString = (angle: number) => 'rotate3d(1, 0, 0, ' + angle + 'deg)';

export const redColorStyle = style({ color: 'red' });

export const synchronousRotation3dAnimation = (duration: number, colors: { [key: string]: string }, isInverseRotation: boolean) => {
    return propertyAnimation(duration, [
        { color: colors['first'], transform: rotate3dAngleToString(0) },
        { color: colors['third'], transform: rotate3dAngleToString(isInverseRotation ? -90 : 90) },
        { color: colors['second'], transform: rotate3dAngleToString(!isInverseRotation ? 180 : -180) },
        { color: colors['first'], transform: rotate3dAngleToString(isInverseRotation ? -270 : 270) },
        { color: colors['third'], transform: rotate3dAngleToString(!isInverseRotation ? 360 : -360) }
    ]);
};