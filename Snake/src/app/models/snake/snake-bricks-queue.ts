import * as Collections from 'typescript-collections';
import { StandardBrick } from '../brick/standard-brick';

export class SnakeBricksQueue {

    private _list: Collections.LinkedList<StandardBrick> = new Collections.LinkedList<StandardBrick>();

    get head(): StandardBrick | undefined {
        return this._list.last();
    }

    get tail(): StandardBrick | undefined {
        return this._list.first();
    }

    get length(): number {
        return this._list.size();
    }

    clear(): void {
        this._list.clear();
    }

    dequeue(): StandardBrick | undefined {
        let tailBrick: StandardBrick = this.tail;

        this._list.removeElementAtIndex(0);

        return tailBrick;
    }

    enqueue(brick: StandardBrick): void {
        this._list.add(brick);
    }
}