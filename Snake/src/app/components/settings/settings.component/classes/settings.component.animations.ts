import { query, stagger, transition, trigger } from "@angular/animations";
import { directOneWayOpacityAnimation } from "src/app/components/animations";
import { settingsOptionChangeButtonsDisabledStyle, settingsOptionComponentAnimation } from "../../settings.option.component/classes/settings-option.component.animations";

const appearanceAnimationDuration = 502;

const settingsOptionStaggerAnimationTiming = 278;

const mainContainerAnimations = [
    trigger('mainContainerTrigger', [
        transition('void => *', [
            settingsOptionChangeButtonsDisabledStyle,
            directOneWayOpacityAnimation(appearanceAnimationDuration),
            query('settings-option-root', stagger(settingsOptionStaggerAnimationTiming, settingsOptionComponentAnimation))
        ])
    ])
];

export const settingsComponentAnimations = [mainContainerAnimations];