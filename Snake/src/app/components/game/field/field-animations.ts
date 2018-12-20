import { propertyAnimation } from "../../animations";

export const twoWayPropertyAnimation = (duration: number, propertyName: string, firstColor: string, secondColor: string) =>
    propertyAnimation(duration, [
        { [propertyName]: firstColor },
        { [propertyName]: secondColor },
        { [propertyName]: firstColor },
        { [propertyName]: secondColor },
        { [propertyName]: firstColor }
    ]);