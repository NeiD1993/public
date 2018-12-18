import { GameState } from "src/app/enums/game/game-state";
import { ComponentState } from "src/app/enums/component-state";

export class ActivatableGameComponentViewState {

    getExtendedComponentState(componentState: ComponentState, gameState: GameState): string {
        if (componentState != ComponentState.ActiveDisplayed)
            return ComponentState[componentState];
        else
            return GameState[gameState];
    }
}