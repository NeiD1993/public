import * as Collections from 'typescript-collections';
import { FieldBrick } from "./brick/field-brick";
import { MathService } from '../services/math.service';
import { StandardBrick } from './brick/standard-brick';

export class Bricks {

    private static readonly _minSideBricksCount: number = 10;

    private _sideBricksCount: number;

    private _elements: Array<Array<FieldBrick>> = new Array<Array<FieldBrick>>();

    private _freeElementsIndexes: Collections.Set<number> = new Collections.Set<number>();

    set sideBricksCount(sideBricksCount: number) {
        this._sideBricksCount = (sideBricksCount >= Bricks._minSideBricksCount) ? sideBricksCount : Bricks._minSideBricksCount;
        this.clearElements();
        this.createElements();
        this._freeElementsIndexes.clear();
    }

    get sideBricksCount(): number {
        return this._sideBricksCount;
    }

    get elements(): Array<Array<FieldBrick>> {
        return this._elements;
    }

    get freeElementsIndexes(): Collections.Set<number> {
        return this._freeElementsIndexes;
    }

    private clearElements(): void {
        this._elements.forEach(fieldBrickArray => (fieldBrickArray.length = 0));
        this._elements.length = 0;
    }

    private createElements(): void {
        this._elements.length = this._sideBricksCount;

        for (let rowIndex = 0; rowIndex < this.sideBricksCount; rowIndex++) {
            this.elements[rowIndex] = new Array<FieldBrick>();
            this.elements[rowIndex].length = this.sideBricksCount;
        }
    }

    private isBrickSideIndexCorrect(sideIndex: number): boolean {
        return ((sideIndex >= 0) && (sideIndex < this.sideBricksCount));
    }

    getBrickIndex(brick: StandardBrick): number {
        return (brick.x + this.sideBricksCount * brick.y);
    }

    getBrick(index: number): StandardBrick {
        let x: number = index % this.sideBricksCount;
        let y: number = MathService.roundNumbersDivision(index, this.sideBricksCount);

        return this.elements[x][y];
    }

    getSideBricksCountPartLength(partsCount: number): number {
        if (partsCount >= 1)
            return MathService.roundNumbersDivision(this.sideBricksCount, partsCount);
    }

    getTotalBricksCount(): number {
        return MathService.powerNumber(this.sideBricksCount, 2);
    }

    isBrickCorrect(brick: StandardBrick): boolean {
        return (this.isBrickSideIndexCorrect(brick.x) && this.isBrickSideIndexCorrect(brick.y));
    }
}