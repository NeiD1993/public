import { ActivatableGameComponentViewState } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { Difficulty } from "src/app/enums/difficulty";
import { IBrickDecorationService } from "src/app/services/brick-decoration/i-brick-decoration.service";

export class FieldComponentViewState extends ActivatableGameComponentViewState {

    readonly fieldContainerBoundaryLength: number;

    readonly fieldContainerSideLength: number;

    readonly fieldSideLength: number;

    constructor(bricksDecorationService: IBrickDecorationService, difficulty: Difficulty, sideBricksCount: number) {
        super();
        this.fieldSideLength = sideBricksCount * (bricksDecorationService.getBrickSideLength(difficulty) +
            2 * bricksDecorationService.getBrickMarginLength(difficulty));
        this.fieldContainerSideLength = this.fieldSideLength + 2 * bricksDecorationService.getBoundaryBrickSpaceLength(difficulty);
        this.fieldContainerBoundaryLength = bricksDecorationService.getBoundaryLength(difficulty);
    }
}