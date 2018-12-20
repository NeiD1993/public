import { inverseOneWayOpacityAnimation } from "../../game-animations";
import { propertyAnimation } from "src/app/components/animations";
import { sequence, query } from "@angular/animations";

const animationDuration = 1215;

const halfAnimationDuration = animationDuration / 2;

export const loadingComponentDestroingAnimation = sequence([
    query('h1',
        propertyAnimation(halfAnimationDuration, [
            { color: 'black' },
            { color: 'green' }
        ])
    ),
    inverseOneWayOpacityAnimation(halfAnimationDuration)
]);