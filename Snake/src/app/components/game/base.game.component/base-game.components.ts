import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { ChangeDetectorRef, OnInit } from "@angular/core";

export abstract class BaseGameComponent {

    constructor(protected _changeDetector: ChangeDetectorRef, public gameLogicService: BaseGameLogicService) { }

    update(): void {
        this._changeDetector.detectChanges();
    }
}

export class BaseDetachedGameComponent extends BaseGameComponent implements OnInit {

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService) {
        super(changeDetector, gameLogicService);
    }

    ngOnInit(): void {
        this._changeDetector.detach();
    }
}