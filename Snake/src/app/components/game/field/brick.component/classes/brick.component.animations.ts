import { AnimationMetadata, query, sequence, state, style, transition, trigger } from "@angular/animations";
import { greenBackgroundColorStyle, redBackgroundColorStyle } from "../../../game-animations";
import { newFoodGeneratedAnimationDuration } from "../../field.component/classes/field.component.animations";
import { propertyAnimation } from "src/app/components/animations";
import { twoWayPropertyAnimation } from "../../field-animations";

const brickAnimation = (animationMetadatas: AnimationMetadata[]) => query('.brick', animationMetadatas);

const brickContainerAnimation = (duration: string, horizontalAlign: string, verticalAlign: string,
    heightParameters: { start: string | number, end: string | number },
    widthParameters: { start: string | number, end: string | number }) => sequence([
        style({ alignItems: verticalAlign, justifyContent: horizontalAlign }),
        brickAnimation([
            redBackgroundColorStyle,
            brickSideAnimationMetadata(
                duration,
                { start: heightParameters.start, end: heightParameters.end },
                { start: widthParameters.start, end: widthParameters.end }
            )
        ]),
        style({ alignItems: 'center', justifyContent: 'center' })
    ]);

const brickSideAnimationMetadata = (duration: string, heightParameters: { start: string | number, end: string | number },
    widthParameters: { start: string | number, end: string | number }) =>
    propertyAnimation(duration, [
        { height: heightParameters.start, width: widthParameters.start },
        { height: heightParameters.end, width: widthParameters.end }
    ]);

const brickAnimations = [
    trigger('brickContainerTrigger', [
        transition('Empty => Food', brickAnimation([
            twoWayPropertyAnimation(newFoodGeneratedAnimationDuration, 'backgroundColor', 'white', 'green')
        ])),
        transition('Empty <=> Snake', brickContainerAnimation('{{duration}}', '{{horizontalAlign}}', '{{verticalAlign}}',
            { start: '{{startHeight}}', end: '{{endHeight}}' },
            { start: '{{startWidth}}', end: '{{endWidth}}' }))
    ]),
    trigger('brickTrigger', [
        state('Empty', style({ backgroundColor: 'white' })),
        state('Food', greenBackgroundColorStyle),
        state('Snake', redBackgroundColorStyle)
    ])
];

export const brickComponentAnimations = [brickAnimations];