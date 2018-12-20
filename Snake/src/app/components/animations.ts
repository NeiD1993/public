import { animate, keyframes, style } from "@angular/animations";

export const directOneWayOpacityAnimation = (duration: number) => propertyAnimation(duration, [
    { opacity: 0 },
    { opacity: 1 }
]);

export const disabledPointerEventsStyle = style({ pointerEvents: 'none' });

export const fullRotation3dAnimation = (duration: number) =>
    propertyAnimation(duration, [
        { transform: rotate3dAngleToString(0) },
        { transform: rotate3dAngleToString(360) }
    ]);

export const propertyAnimation = (duration: string | number, values: Array<{ [key: string]: string | number }>) => {
    let offsetStep: number = 1 / (values.length - 1);
    let result = animate(duration,
        keyframes(values.map((value, index) => {
            value['offset'] = index * offsetStep;

            return style(value);
        }))
    );

    return result;
}

export const rotate3dAngleToString = (angle: number) => 'rotate3d(1, 0, 0, ' + angle + 'deg)';

export const twoWayOpacityAnimation = (duration: number) => propertyAnimation(duration, [
    { opacity: 1 },
    { opacity: 0 },
    { opacity: 1 }
]);