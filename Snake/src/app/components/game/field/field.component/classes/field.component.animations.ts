import { AnimationMetadata, query, group, state, style, transition, trigger } from "@angular/animations";
import { backgroundTricolorAnimation, gameStateTrigger, nonFinishedGameAnimationDuration } from "../../../game-animations";
import { directOneWayOpacityAnimation, propertyAnimation, twoWayOpacityAnimation } from "src/app/components/animations";
import { twoWayPropertyAnimation } from "../../field-animations";

const brickTransformationBackgroundColorAnimation = (duration: number, color: string) => backgroundTricolorAnimation(duration, color, color);

const foodEatenAnimationDuration = 385;

export const newFoodGeneratedAnimationDuration = 750;

const notStartedBrickTransformationAnimationDuration = 850;

export const snakeBricksAnimation = (animationFunction: AnimationMetadata) => query('.Snake', animationFunction);

const twoWayFoodEatenAnimation = (duration: number, propertyName: string) => twoWayPropertyAnimation(duration, propertyName, 'blue', 'red');

const fieldAnimations = [
    gameStateTrigger('gameStateTrigger', [
        state('Paused', style({ animation: 'directOpacityChanging 2s Infinite', borderColor: 'blue' })),
        state('Started', style({ borderColor: 'red' })),
        transition('NotDisplayed => NotStarted', [
            style({ borderColor: 'black' }),
            twoWayOpacityAnimation(nonFinishedGameAnimationDuration),
            group([
                query('.Food', brickTransformationBackgroundColorAnimation(notStartedBrickTransformationAnimationDuration, 'green')),
                snakeBricksAnimation(brickTransformationBackgroundColorAnimation(notStartedBrickTransformationAnimationDuration, 'red'))
            ])
        ]),
        transition('Paused => Started', directOneWayOpacityAnimation(nonFinishedGameAnimationDuration)),
        transition('NotStarted => Started', twoWayOpacityAnimation(nonFinishedGameAnimationDuration))
    ]),
    trigger('foodStatusTrigger', [
        transition('Eaten => Fresh', [
            propertyAnimation(newFoodGeneratedAnimationDuration, [
                { borderColor: 'red' },
                { borderColor: 'green' },
                { borderColor: 'red' }
            ])
        ]),
        transition('Fresh => Eaten', group([
            twoWayFoodEatenAnimation(foodEatenAnimationDuration, 'borderColor'),
            snakeBricksAnimation(twoWayFoodEatenAnimation(foodEatenAnimationDuration, 'backgroundColor'))
        ]))
    ])
];

export const fieldComponentAnimations = [fieldAnimations];