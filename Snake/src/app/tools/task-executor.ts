import * as time from 'util-time';

export class TaskExecutor {

    private static readonly _minDelay: number = 65;

    private _delay: number = TaskExecutor._minDelay;

    private _executor: time.Timer;

    private _isPaused: boolean = false;

    private _isStarted: boolean = false;

    constructor() {
        this._executor = new time.Timer();
    }

    set delay(delay: number) {
        this._delay = (delay > TaskExecutor._minDelay) ? delay : TaskExecutor._minDelay;
        this.reset();
    }

    continue(): void {
        if (this._isPaused) {
            this._executor.continue();
            this._isPaused = false;
        }
    }

    pause(): void {
        if (this._isStarted && !this._isPaused) {
            this._executor.pause();
            this._isPaused = true;
        }
    }

    reset(): void {
        if (this._isStarted) {
            this._executor.reset();
            this._isStarted = this._isPaused = false;
        }
    }

    start(task: () => void): void {
        this.reset();
        this._executor.start(this._delay, task);
        this._isStarted = true;
    }
}