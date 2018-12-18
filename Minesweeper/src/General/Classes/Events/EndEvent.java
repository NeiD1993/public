package General.Classes.Events;

import General.Enums.EndStates;

import java.util.EventObject;

/**
 * Created by NeiD on 05.09.2016.
 */
public class EndEvent extends EventObject {

    protected EndStates endState;

    public EndEvent(Object source, EndStates endState) {
        super(source);
        if (endState != null)
            this.endState = endState;
    }

    public EndStates getEndState() {
        return endState;
    }

}
