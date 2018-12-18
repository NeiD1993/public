import { AnimationMetadata } from "@angular/animations";
import { BaseAnimationServiceGameComponentViewState } from "../../base.game.component/base-animation-service-game.component.view-state";
import { ComponentState } from "src/app/enums/component-state";
import { gameComponentFieldFinishedAnimation, gameComponentStartButtonFinishedAnimation } from "./game.component.animations";
import { loadingComponentDestroingAnimation } from "../../loading.component/classes/loading.component.animations";

export class GameComponentViewState extends BaseAnimationServiceGameComponentViewState {

    protected createAnimations(): Array<AnimationMetadata> {
        return [
            gameComponentFieldFinishedAnimation,
            gameComponentStartButtonFinishedAnimation,
            loadingComponentDestroingAnimation
        ];
    }

    getGameEntityAnimation(gameEntityType: string): AnimationMetadata {
        if (gameEntityType == GameEntityType[GameEntityType.Field])
            return gameComponentFieldFinishedAnimation;
        else if (gameEntityType == GameEntityType[GameEntityType.LoadingContainer])
            return loadingComponentDestroingAnimation;
        else
            return gameComponentStartButtonFinishedAnimation;
    }

    getGameInitializationAttemptsCountRestNumberType(gameInitializationAttemptsCountRest: number): string {
        return (gameInitializationAttemptsCountRest % 2 == 0) ?
            GameInitializationAttemptsCountRestNumberType[GameInitializationAttemptsCountRestNumberType.Even] :
            GameInitializationAttemptsCountRestNumberType[GameInitializationAttemptsCountRestNumberType.Odd];
    }

    getOutFieldContainerDisplayType(state: ComponentState): string {
        return (state == ComponentState.NotDisplayed) ?
            OutFieldContainerDisplayType[OutFieldContainerDisplayType.None] : OutFieldContainerDisplayType[OutFieldContainerDisplayType.Flex];
    }
}

enum GameEntityType {

    Field,

    LoadingContainer,

    StartButton
}

enum GameInitializationAttemptsCountRestNumberType {

    Even,

    Odd
}

enum OutFieldContainerDisplayType {

    Flex,

    None
}