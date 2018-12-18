import { AnimationMetadata, AnimationStyleMetadata, group, query, sequence, state, style, transition, trigger } from '@angular/animations';
import { backgroundBicolorAnimation, blackBackgroundColorStyle, blueBackgroundColorStyle, directOneWayOpacityAnimation, finishedGameAnimationDuration, gameStateTrigger, greenBackgroundColorStyle, inverseOneWayOpacityAnimation, propertyAnimation, redBackgroundColorStyle, rotateAngleToString, twoWayOpacityAnimation } from '../../../game-animations';
import { blueColorStyle, fullRotation3dAnimation, greenColorStyle, redColorStyle, synchronousRotation3dAnimation } from '../../snake-dashboard.component.animations';
import { BonusType } from 'src/app/enums/bonus-type';

const animationDuration = 300;

const arrowAnimationMetadata = (stateStyles: { [outerKey: string]: AnimationStyleMetadata }, animationFunction: (firstColor: string,
    secondColor: string, params: { firstAngle: number, secondAngle: number } | { intermediateColor: string, isDirectAnimation: boolean }) =>
    AnimationMetadata,
    params: { [paramKey: string]: { firstAngle: number, secondAngle: number } | { intermediateColor: string, isDirectAnimation: boolean } }) =>
    Object.keys(stateStyles).map(key => <AnimationMetadata>state(key, stateStyles[key])).concat([
        transition('Down => Left', animationFunction('red', 'black', params['downLeft'])),
        transition('Down => Right', animationFunction('red', 'green', params['downRight'])),
        transition('Down => Up', animationFunction('red', 'blue', params['downUp'])),
        transition('Left => Down', animationFunction('black', 'red', params['leftDown'])),
        transition('Left => Right', animationFunction('black', 'green', params['leftRight'])),
        transition('Left => Up', animationFunction('black', 'blue', params['leftUp'])),
        transition('Right => Down', animationFunction('green', 'red', params['rightDown'])),
        transition('Right => Left', animationFunction('green', 'black', params['rightLeft'])),
        transition('Right => Up', animationFunction('green', 'blue', params['rightUp'])),
        transition('Up => Down', animationFunction('blue', 'red', params['upDown'])),
        transition('Up => Left', animationFunction('blue', 'black', params['upLeft'])),
        transition('Up => Right', animationFunction('blue', 'green', params['upRight']))
    ]);

const boundaryAnimationDuration = 550;

const compassAnimation = (opacityAnimation: (duration: number) => AnimationMetadata) =>
    group([
        sequence([
            query('.compass',
                group([
                    opacityAnimation(animationDuration),
                    query('.boundary',
                        propertyAnimation(boundaryAnimationDuration, [
                            { borderColor: 'red' },
                            { borderColor: 'green' },
                            { borderColor: 'blue' },
                            { borderColor: 'black' }
                        ])
                    )
                ])
            ),
            query('.directionTextContainer', fullRotation3dAnimation(directionTextContainerAnimationDuration, false))
        ]),
        query('.speedLevelTextContainer', fullRotation3dAnimation(directionTextContainerAnimationDuration / 2, true))
    ])

const directionTextContainerAnimationDuration = 505;

const textContainerTextAnimationDuration = 350;

const textContainerTextAnimationDoubleDuration = 2 * textContainerTextAnimationDuration;

const gameStateAnimations = [
    gameStateTrigger('snakeDirectionGameStateTrigger', [
        transition('* => Finished', inverseOneWayOpacityAnimation(finishedGameAnimationDuration)),
        transition('* => NonActive, NonActive => NotStarted, NotDisplayed => NotStarted', compassAnimation(twoWayOpacityAnimation)),
        transition('NotStarted => Started, Paused <=> Started', compassAnimation(directOneWayOpacityAnimation))
    ])
];

const compassAnimations = [
    trigger('snakeDirectionArrowTrigger',
        arrowAnimationMetadata(
            {
                'Down': style({ transform: 'rotate(180deg)' }),
                'Left': style({ transform: 'rotate(270deg)' }),
                'Right': style({ transform: 'rotate(90deg)' }),
                'Up': style({ transform: 'rotate(0deg)' })
            },
            (firstColor: string, secondColor: string, params: { firstAngle: number, secondAngle: number }) =>
                group([
                    query('.arrowTop', propertyAnimation(animationDuration, [
                        { borderBottomColor: firstColor },
                        { borderBottomColor: secondColor }
                    ])),
                    query('.arrowBottom', backgroundBicolorAnimation(animationDuration, firstColor, secondColor)),
                    propertyAnimation(animationDuration, [
                        { transform: rotateAngleToString(params.firstAngle) },
                        { transform: rotateAngleToString(params.secondAngle) }
                    ])
                ]),
            {
                'downLeft': { firstAngle: 180, secondAngle: 270 },
                'downRight': { firstAngle: 180, secondAngle: 90 },
                'downUp': { firstAngle: 180, secondAngle: 360 },
                'leftDown': { firstAngle: 270, secondAngle: 180 },
                'leftRight': { firstAngle: 270, secondAngle: 90 },
                'leftUp': { firstAngle: 270, secondAngle: 360 },
                'rightDown': { firstAngle: 90, secondAngle: 180 },
                'rightLeft': { firstAngle: 90, secondAngle: 270 },
                'rightUp': { firstAngle: 90, secondAngle: 0 },
                'upDown': { firstAngle: 0, secondAngle: 180 },
                'upLeft': { firstAngle: 0, secondAngle: -90 },
                'upRight': { firstAngle: 0, secondAngle: 90 },
            }
        )
    ),
    trigger('snakeDirectionArrowTopTrigger', [
        state('Down', style({ borderBottomColor: 'red' })),
        state('Left', style({ borderBottomColor: 'black' })),
        state('Right', style({ borderBottomColor: 'green' })),
        state('Up', style({ borderBottomColor: 'blue' }))
    ]),
    trigger('snakeDirectionArrowBottomTrigger', [
        state('Down', redBackgroundColorStyle),
        state('Left', blackBackgroundColorStyle),
        state('Right', greenBackgroundColorStyle),
        state('Up', blueBackgroundColorStyle)
    ])
]

const textContainerAnimations = [
    trigger('snakeDirectionSnakeDirectionTextTrigger',
        arrowAnimationMetadata(
            {
                'Down': redColorStyle,
                'Left': style({ color: 'black' }),
                'Right': greenColorStyle,
                'Up': blueColorStyle
            },
            (firstColor: string, secondColor: string, params: { intermediateColor: string, isDirectAnimation: boolean }) =>
                synchronousRotation3dAnimation(textContainerTextAnimationDuration, {
                    'first': firstColor,
                    'second': params.intermediateColor,
                    'third': secondColor
                }, params.isDirectAnimation),
            {
                'downLeft': { intermediateColor: 'green', isDirectAnimation: true },
                'downRight': { intermediateColor: 'black', isDirectAnimation: true },
                'downUp': { intermediateColor: 'green', isDirectAnimation: true },
                'leftDown': { intermediateColor: 'blue', isDirectAnimation: false },
                'leftRight': { intermediateColor: 'red', isDirectAnimation: false },
                'leftUp': { intermediateColor: 'green', isDirectAnimation: false },
                'rightDown': { intermediateColor: 'blue', isDirectAnimation: false },
                'rightLeft': { intermediateColor: 'blue', isDirectAnimation: true },
                'rightUp': { intermediateColor: 'red', isDirectAnimation: true },
                'upDown': { intermediateColor: 'black', isDirectAnimation: false },
                'upLeft': { intermediateColor: 'red', isDirectAnimation: true },
                'upRight': { intermediateColor: 'black', isDirectAnimation: false }
            }
        )
    )
];

export const snakeDirectionDashboardComponentAnimations = [
    gameStateAnimations,
    compassAnimations,
    textContainerAnimations
];

export const snakeDirectionSnakeSpeedLevelTextChangedAnimations = new Map<BonusType, AnimationMetadata>([
    [BonusType.LevelDown, fullRotation3dAnimation(textContainerTextAnimationDoubleDuration, false)],
    [BonusType.LevelUp, fullRotation3dAnimation(textContainerTextAnimationDoubleDuration, true)]
]);