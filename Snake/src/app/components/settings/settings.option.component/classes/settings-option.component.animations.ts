import { AnimationMetadata, query, state, transition, trigger } from "@angular/animations";
import { disabledPointerEventsStyle, fullRotation3dAnimation, twoWayOpacityAnimation } from "src/app/components/animations";

export const settingsOptionChangeButtonsDisabledStyle = query('.topChangeButton, .bottomChangeButton', disabledPointerEventsStyle);

const settingsOptionContainerAppearanceAnimationDuration = 572;

const settingsOptionTextAnimation = (animation: AnimationMetadata) => query('.optionText h1', animation);

const settingsOptionTextAnimationDuration = 372;

export const settingsOptionComponentAnimation = twoWayOpacityAnimation(settingsOptionContainerAppearanceAnimationDuration);

const settingsOptionAnimations = [
    trigger('settingsOptionTrigger', [
        transition('* => *', [
            settingsOptionChangeButtonsDisabledStyle,
            settingsOptionTextAnimation(fullRotation3dAnimation(settingsOptionTextAnimationDuration))
        ])
    ]),
    trigger('changeButtonTrigger', [state('Disabled', disabledPointerEventsStyle)])
];

export const settingsOptionComponentAnimations = [settingsOptionAnimations];