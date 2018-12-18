package app.controllers;

import app.controllers.base.BaseController;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;

/**
 * Created by NeiD on 23.11.2016.
 */
@Controller
public class MainController extends BaseController {

    @RequestMapping(value = "/", method = RequestMethod.GET)
    public String main() {
        logger.debug("Received request to show main");
        return "mainView";
    }

}
