import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ChangeDetectorRef } from "@angular/core";

export abstract class BaseGameComponent {

    constructor(protected _changeDetector: ChangeDetectorRef, public gameLogicService: BaseGameLogicService, isDetached: boolean) {
        if (isDetached)
            this._changeDetector.detach();
    }

    update(): void {
        this._changeDetector.detectChanges();
    }
}

export abstract class BaseViewStateComponent<T> extends BaseGameComponent {

    protected _viewState: T;

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService, isDetached: boolean) {
        super(changeDetector, gameLogicService, isDetached);
    }

    get viewState(): T {
        return this._viewState;
    }

    protected abstract createViewState(): T;

    ngOnInit(): void {
        this._viewState = this.createViewState();
    }
}