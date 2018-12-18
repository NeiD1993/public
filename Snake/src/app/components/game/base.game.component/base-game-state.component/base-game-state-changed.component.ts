import { BaseActivatableGameComponent } from "../base-activatable-game.component/base-activatable-game.component";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { GameState } from "src/app/enums/game/game-state";
import { Subscription } from "rxjs";

export abstract class BaseGameStateChangedComponent extends BaseActivatableGameComponent {

    protected abstract getGameStartedAnimationPerformedValue(animatedElement: any): any;

    protected generateGameEventsSubscriptions(): Map<GameEventType, Subscription> {
        let gameEventsSubsriptions: Map<GameEventType, Subscription> = new Map<GameEventType, Subscription>();

        gameEventsSubsriptions.set(GameEventType.GameStateChanged, this.gameLogicService.gameStateChanged.subscribe(this.onGameStateChanged.bind(this)));

        return gameEventsSubsriptions;
    }

    onGameStateChanged(gameState: GameState): void {
        this.update();
    }
}