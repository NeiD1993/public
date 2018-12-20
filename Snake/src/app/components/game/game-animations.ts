import { AnimationMetadata, state, style, trigger } from "@angular/animations";
import { propertyAnimation } from "../animations";

export const backgroundBicolorAnimation = (duration: number, firstColor: string, secondColor: string) =>
    propertyAnimation(duration, [
        { backgroundColor: firstColor },
        { backgroundColor: secondColor }
    ]);

export const backgroundTricolorAnimation = (duration: number, firstColor: string, secondColor: string) =>
    propertyAnimation(duration, [
        { backgroundColor: firstColor },
        { backgroundColor: 'black' },
        { backgroundColor: secondColor }
    ]);

export const blackBackgroundColorStyle = style({ backgroundColor: 'black' });

export const blueBackgroundColorStyle = style({ backgroundColor: 'blue' });

export const directHalfRotationAnimation = (duration: number) => propertyAnimation(duration, [
    { transform: rotateAngleToString(0) },
    { transform: rotateAngleToString(180) },
    { transform: rotateAngleToString(0) }
]);

export const directWidthAnimationMetadata = (duration: number) => propertyAnimation(duration, [
    { width: 0 },
    { width: '100%' }
]);

export const finishedGameAnimationDuration = 1500;

export const gameStateTrigger = (triggerName: string, triggerMetadatas: AnimationMetadata[]) =>
    trigger(triggerName, [<AnimationMetadata>(state('NotDisplayed', style({ display: 'none' })))].concat(triggerMetadatas));

export const greenBackgroundColorStyle = style({ backgroundColor: 'green' });

export const inverseHalfRotationAnimation = (duration: number) => propertyAnimation(duration, [
    { transform: rotateAngleToString(0) },
    { transform: rotateAngleToString(-180) },
    { transform: rotateAngleToString(0) }
]);

export const inverseOneWayOpacityAnimation = (duration: number) => propertyAnimation(duration, [
    { opacity: 1 },
    { opacity: 0 }
]);

export const nonFinishedGameAnimationDuration = 256;

export const redBackgroundColorStyle = style({ backgroundColor: 'red' });

export const rotateAngleToString = (angle: number) => 'rotate(' + angle + 'deg)';