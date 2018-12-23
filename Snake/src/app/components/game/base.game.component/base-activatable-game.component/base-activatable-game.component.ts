import { ActivatableGameComponentViewState } from "./base-activatable-game.component.view-state";
import { BaseViewStateComponent } from "../base-game.components";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ChangeDetectorRef, InjectionToken, OnDestroy } from "@angular/core";
import { ComponentState } from "src/app/enums/component-state";
import { GameEventType } from "src/app/enums/game/game-event-type";
import { Subscription } from "rxjs";
import { SubscriptionsService } from "src/app/services/subscriptions.service";

export abstract class BaseActivatableGameComponent extends BaseViewStateComponent<ActivatableGameComponentViewState> implements OnDestroy {

    private _state: ComponentState = ComponentState.NotDisplayed;

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService,
        protected _gameEventsSubscriptionsService: SubscriptionsService<GameEventType>) {
        super(changeDetector, gameLogicService, true);
        this._gameEventsSubscriptionsService.subscriptions = this.generateGameEventsSubscriptions();
    }

    get state(): ComponentState {
        return this._state;
    }

    get stateToString(): string {
        return ComponentState[this._state];
    }

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