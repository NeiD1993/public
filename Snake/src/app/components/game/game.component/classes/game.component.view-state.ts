import { AnimationMetadata } from "@angular/animations";
import { BaseAnimationServiceGameComponentViewState } from "../../base.game.component/base-animation-service-game.component.view-state";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ComponentState } from "src/app/enums/component-state";
import { gameComponentFieldFinishedAnimation, gameComponentStartButtonFinishedAnimation } from "./game.component.animations";
import { GameState } from "src/app/enums/game/game-state";
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

    getOutFieldDisplayType(state: ComponentState): string {
        return (state == ComponentState.NotDisplayed) ?
            OutFieldContainerDisplayType[OutFieldContainerDisplayType.None] : OutFieldContainerDisplayType[OutFieldContainerDisplayType.Flex];
    }

    getRestartButtonContainerDisplayType(gameState: GameState): string {
        return ((gameState == GameState.NotInitialized) || (gameState != GameState.NotStarted)) ?
            OutFieldContainerDisplayType[OutFieldContainerDisplayType.Flex] : OutFieldContainerDisplayType[OutFieldContainerDisplayType.None];
    }

    getRestartButtonDisplayType(gameLogicService: BaseGameLogicService, state: ComponentState): string {
        return (gameLogicService.gameState != GameState.NotInitialized) ?
            this.getExtendedComponentState(state, gameLogicService.gameState) :
            this.getGameInitializationAttemptsCountRestNumberType(gameLogicService.gameInitializationAttemptsCountRest);
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