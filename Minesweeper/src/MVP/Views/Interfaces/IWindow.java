package MVP.Views.Interfaces;

import java.awt.*;

/**
 * Created by NeiD on 10.09.2016.
 */
public interface IWindow<DisplayClass extends Graphics> {

    void setWindowVisible(boolean isWindowVisible);

    void setWindowResizable(boolean isWindowResizable);

    void setWindowLayout(LayoutManager layoutManager);

    void addFieldViewAndFieldViewConstraints(IFieldView<DisplayClass> fieldView, Object fieldViewConstraints);

    void addControlPanelAndControlPanelConstraints(IControlPanel controlPanel, Object controlPanelConstraints);

}
