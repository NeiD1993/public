import { ActivatableGameComponentViewState } from "./base-activatable-game.component.view-state";
import { BaseDetachedGameComponent } from "../base-game.components";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ChangeDetectorRef, InjectionToken, OnDestroy } from "@angular/core";
import { ComponentState } from "src/app/enums/component-state";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { Subscription } from "rxjs";
import { SubscriptionsService } from "src/app/services/subscriptions.service";

export abstract class BaseActivatableGameComponent extends BaseDetachedGameComponent implements OnDestroy {

    private _state: ComponentState = ComponentState.NotDisplayed;

    protected _viewState: ActivatableGameComponentViewState;

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService, 
        protected _gameEventsSubscriptionsService: SubscriptionsService<GameEventType>) {
        super(changeDetector, gameLogicService);
        this._gameEventsSubscriptionsService.subscriptions = this.generateGameEventsSubscriptions();
    }

    get state(): ComponentState {
        return this._state;
    }

    get stateToString(): string {
        return ComponentState[this._state];
    }

    get viewState(): ActivatableGameComponentViewState {
        return this._viewState;
    }

    protected abstract createViewState(): ActivatableGameComponentViewState;

    protected abstract generateGameEventsSubscriptions(): Map<GameEventType, Subscription>;

    deactivate(): void {
        this._state = ComponentState.NonActive;
    }

    display(): void {
        this._state = ComponentState.ActiveDisplayed;
    }

    hide(): void {
        this._state = ComponentState.NotDisplayed;
    }

    ngOnDestroy(): void {
        this._gameEventsSubscriptionsService.resetSubscriptions();
    }

    ngOnInit(): void {
        super.ngOnInit();
        this._viewState = this.createViewState();
    }
}

export let GameEventsSubscriptionsServiceToken: InjectionToken<any> = new InjectionToken('GameEventsSubscriptionsService');