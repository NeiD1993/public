import { animateChild, AnimationAnimateMetadata, AnimationStyleMetadata, group, query, sequence, state, transition, trigger } from '@angular/animations';
import { backgroundBicolorAnimation, backgroundTricolorAnimation, blackBackgroundColorStyle, blueBackgroundColorStyle, directHalfRotationAnimation, directWidthAnimationMetadata, finishedGameAnimationDuration, gameStateTrigger, greenBackgroundColorStyle, inverseHalfRotationAnimation, inverseOneWayOpacityAnimation, redBackgroundColorStyle } from '../../../game-animations';
import { blueColorStyle, greenColorStyle, redColorStyle, synchronousRotation3dAnimation } from '../../snake-dashboard.component.animations';
import { directOneWayOpacityAnimation, fullRotation3dAnimation, propertyAnimation, twoWayOpacityAnimation } from 'src/app/components/animations';

const actionIndicatorAnimationDuration = 1475;

const actionIndicatorAnimation = (colorStyle: AnimationStyleMetadata) =>
    sequence([
        colorStyle,
        propertyAnimation(actionIndicatorAnimationDuration, [
            { opacity: 1 },
            { opacity: 0 },
            { opacity: 1 },
            { opacity: 0 },
            { opacity: 1 },
            { opacity: 0 },
            { opacity: 1 }
        ])
    ]);

const animationDuration = 405;

const gameStateAnimation = (opacityAnimation: (duration: number) => AnimationAnimateMetadata,
    sliderBackgroundColor: string, limiterBackgroundColor: string) => [
        group([
            sequence([
                query('.sliderContainer, .limiterContainer, .rotatingBlock', opacityAnimation(animationDuration)),
                group([
                    query('.sliderCircle', backgroundTricolorAnimation(animationDuration, sliderBackgroundColor, sliderBackgroundColor)),
                    query('.limiter',
                        group([
                            backgroundBicolorAnimation(animationDuration, 'red', limiterBackgroundColor),
                            directWidthAnimationMetadata(animationDuration)
                        ])
                    ),
                    query('.rotatingBlock', inverseHalfRotationAnimation(animationDuration))
                ]),
                query('.snakeDashboardTextContainer', fullRotation3dAnimation(animationDuration))
            ]),
            query('@snakeLengthSliderCircleBackgroundColorTrigger, @snakeLengthSnakeLengthTextTrigger, @snakeLengthFoodCountTextTrigger', animateChild())
        ])
    ];

const textContainerAnimation = (firstColor: string, secondColor: string, thirdColor: string) =>
    synchronousRotation3dAnimation(textContainerTextAnimationDuration, {
        'first': firstColor,
        'second': secondColor,
        'third': thirdColor
    });

const textContainerTextAnimationDuration = 650;

const twoWayWidthAnimation = propertyAnimation(animationDuration, [
    { width: '100%' },
    { width: 0 },
    { width: '100%' }
]);

const limiterAnimation = (firstColor: string, secondColor: string) => group([
    backgroundBicolorAnimation(animationDuration, firstColor, secondColor),
    twoWayWidthAnimation
]);

const gameStateAnimations = [
    gameStateTrigger('snakeLengthGameStateTrigger', [
        transition('* => Finished', inverseOneWayOpacityAnimation(finishedGameAnimationDuration)),
        transition('* => NonActive, NonActive => NotStarted, NotDisplayed => NotStarted',
            gameStateAnimation(twoWayOpacityAnimation, '{{sliderBackgroundColor}}', '{{limiterBackgroundColor}}')),
        transition('Paused <=> Started, NotStarted => Started',
            gameStateAnimation(directOneWayOpacityAnimation, '{{sliderBackgroundColor}}', '{{limiterBackgroundColor}}'))
    ])
];

const sliderAnimations = [
    trigger('snakeLengthSliderCircleBackgroundColorTrigger', [
        state('Even', blueBackgroundColorStyle),
        state('Odd', redBackgroundColorStyle),
        transition('Even => Odd', backgroundTricolorAnimation(animationDuration, 'blue', 'red')),
        transition('Odd => Even', backgroundTricolorAnimation(animationDuration, 'red', 'blue'))
    ])
];

const limiterAnimations = [
    trigger('snakeLengthLimiterTrigger', [
        state('Eaten', blackBackgroundColorStyle),
        state('Fresh', greenBackgroundColorStyle),
        transition('Eaten => Fresh', limiterAnimation('black', 'green')),
        transition('Fresh => Eaten', limiterAnimation('green', 'black'))
    ])
];

const rotatingBlockAnimations = [
    trigger('snakeLengthRotatingBlockTrigger', [transition('Fresh => Eaten', directHalfRotationAnimation(animationDuration))])
];

const textContainerAnimations = [
    trigger('snakeLengthSnakeLengthTextTrigger', [
        state('Even', greenColorStyle),
        state('Initial', redColorStyle),
        state('Odd', blueColorStyle),
        transition('Even => Initial', textContainerAnimation('green', 'blue', 'red')),
        transition('Even => Odd', textContainerAnimation('green', 'red', 'blue')),
        transition('Initial => Even', textContainerAnimation('red', 'blue', 'green')),
        transition('Initial => Odd', textContainerAnimation('red', 'green', 'blue')),
        transition('Odd => Even', textContainerAnimation('blue', 'red', 'green')),
        transition('Odd => Initial', textContainerAnimation('blue', 'green', 'red'))
    ]),
    trigger('snakeLengthFoodCountTextTrigger', [
        state('Even', blueColorStyle),
        state('Initial', greenColorStyle),
        state('Odd', redColorStyle),
        transition('Even => Initial', textContainerAnimation('blue', 'red', 'green')),
        transition('Even => Odd', textContainerAnimation('blue', 'green', 'red')),
        transition('Initial => Even', textContainerAnimation('green', 'red', 'blue')),
        transition('Initial => Odd', textContainerAnimation('green', 'blue', 'red')),
        transition('Odd => Even', textContainerAnimation('red', 'green', 'blue')),
        transition('Odd => Initial', textContainerAnimation('red', 'blue', 'green'))
    ])
];

const actionIndicatorAnimations = [
    trigger('snakeLengthActionIndicatorTrigger', [
        state('Eaten', redColorStyle),
        state('Fresh', greenColorStyle)
    ])
]

export const snakeLengthDashboardComponentAnimations = [
    gameStateAnimations,
    sliderAnimations,
    limiterAnimations,
    rotatingBlockAnimations,
    textContainerAnimations,
    actionIndicatorAnimations
];

export const snakeLengthDashboardComponentEatenActionIndicatorAnimation = actionIndicatorAnimation(redColorStyle);

export const snakeLengthDashboardComponentFreshActionIndicatorAnimation = actionIndicatorAnimation(greenColorStyle);