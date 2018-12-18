import { group, query, sequence, state, style, transition, trigger } from '@angular/animations';
import { backgroundBicolorAnimation, blackBackgroundColorStyle, blueBackgroundColorStyle, directHalfRotationAnimation, directOneWayOpacityAnimation, finishedGameAnimationDuration, gameStateTrigger, greenBackgroundColorStyle, inverseHalfRotationAnimation, inverseOneWayOpacityAnimation, nonFinishedGameAnimationDuration, propertyAnimation, redBackgroundColorStyle, rotateAngleToString, twoWayOpacityAnimation } from '../../game-animations';
import { snakeBricksAnimation } from '../../field/field.component/classes/field.component.animations';

const disabledPointerEventsStyle = style({ pointerEvents: 'none' });

const finishedGameAnimationHalfDuration = finishedGameAnimationDuration / 2;

const fullRotationAnimation = propertyAnimation(nonFinishedGameAnimationDuration, [
    { transform: rotateAngleToString(0) },
    { transform: rotateAngleToString(180) },
    { transform: rotateAngleToString(360) },
]);

const transitionButtonStyle = style({ pointerEvents: 'none', height: '70px', width: '70px' });

const restartButtonAnimations = [
    gameStateTrigger('restartButtonTrigger', [
        state('Even', redBackgroundColorStyle),
        state('Finished', blueBackgroundColorStyle),
        state('Odd', greenBackgroundColorStyle),
        transition('* => Even, * => Odd', [
            disabledPointerEventsStyle,
            group([
                fullRotationAnimation,
                directOneWayOpacityAnimation(nonFinishedGameAnimationDuration)
            ])
        ]),
        transition('* => Finished', [
            transitionButtonStyle,
            greenBackgroundColorStyle,
            inverseHalfRotationAnimation(finishedGameAnimationDuration)
        ]),
        transition('Even <=> Odd', [
            disabledPointerEventsStyle,
            fullRotationAnimation
        ]),
        transition("NotStarted <=> Started, Paused => NotStarted, Started <=> Paused", [
            disabledPointerEventsStyle,
            twoWayOpacityAnimation(nonFinishedGameAnimationDuration)
        ])
    ])
];

const emptyContainerAnimations = [
    trigger('emptyTrigger', [
        state('Even', style({ color: 'green' })),
        state('Odd', style({ color: 'red' })),
        transition('void => Even, void => Odd', directOneWayOpacityAnimation(nonFinishedGameAnimationDuration)),
        transition('Even <=> Odd', twoWayOpacityAnimation(nonFinishedGameAnimationDuration))
    ])
];

const startButtonAnimations = [
    gameStateTrigger('startButtonTrigger', [
        state('NonActive', style({ backgroundColor: 'green', pointerEvents: 'none' })),
        state('Paused', blueBackgroundColorStyle),
        state('Started', redBackgroundColorStyle),
        transition('* => NonActive', [
            transitionButtonStyle,
            greenBackgroundColorStyle,
            directHalfRotationAnimation(nonFinishedGameAnimationDuration)
        ]),
        transition('NonActive => NotStarted, NotDisplayed => NotStarted', [
            transitionButtonStyle,
            blackBackgroundColorStyle,
            fullRotationAnimation
        ]),
        transition('NotStarted <=> Started, Paused => NotStarted', [
            transitionButtonStyle,
            directHalfRotationAnimation(nonFinishedGameAnimationDuration)
        ]),
        transition('Paused <=> Started', [
            transitionButtonStyle,
            fullRotationAnimation
        ])
    ])
];

export const gameComponentAnimations = [
    restartButtonAnimations,
    emptyContainerAnimations,
    startButtonAnimations
];

export const gameComponentFieldFinishedAnimation = sequence([
    snakeBricksAnimation(propertyAnimation(finishedGameAnimationHalfDuration, [
        { backgroundColor: 'red' },
        { backgroundColor: 'green' },
        { backgroundColor: 'red' },
        { backgroundColor: 'black' },
        { backgroundColor: 'red' },
        { backgroundColor: 'green' },
        { backgroundColor: 'red' },
        { backgroundColor: 'black' },
        { backgroundColor: 'red' }
    ])),
    snakeBricksAnimation(backgroundBicolorAnimation(finishedGameAnimationHalfDuration, 'red', 'white'))
]);

export const gameComponentStartButtonFinishedAnimation =
    query('.start', [
        transitionButtonStyle,
        blueBackgroundColorStyle,
        group([
            inverseHalfRotationAnimation(finishedGameAnimationDuration),
            inverseOneWayOpacityAnimation(finishedGameAnimationDuration)
        ])
    ]);