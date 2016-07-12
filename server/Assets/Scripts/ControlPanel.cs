using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour {
    public Texture playImage;
    public Texture stopImage;
    public GameObject playKey;
    public RectTransform cursor;
    private RectTransform hoverKey = null;
    
    void Start() {

    }
    
    void Update() {
        updateHover();
        updateColor();
        if (Input.GetButtonUp("Fire1")) {
            confirm();
        }
    }

    void updateHover() {
        foreach (RectTransform key in transform) {
            if (cursorInsideKey(key)) {
                if (hoverKey != key) {
                    hoverKey = key;
                }
                return;
            }
        }
        hoverKey = null;
    }

    void updateColor() {
        foreach (RectTransform key in transform) {
            if (key == hoverKey) {
                setKeyColor(key, Color.yellow);
            }
            else {
                switch (key.name) {
                    /*case "normal":
                        setKeyColor(key, Server.getMethod() == Server.Method.normal ? Color.white : Color.gray);
                        break;
                    case "baseline":
                        setKeyColor(key, Server.getMethod() == Server.Method.baseline ? Color.white : Color.gray);
                        break;
                    case "headOnly":
                        setKeyColor(key, Server.getMethod() == Server.Method.headOnly ? Color.white : Color.gray);
                        break;
                    case "dwell":
                        setKeyColor(key, Server.getMethod() == Server.Method.dwell ? Color.white : Color.gray);
                        break;
                    case "zoonIn":
                        setKeyColor(key, Server.canZoomIn() ? Color.white : Color.gray);
                        break;
                    case "zoonOut":
                        setKeyColor(key, Server.canZoomOut() ? Color.white : Color.gray);
                        break;
                    case "speedUp":
                        setKeyColor(key, Server.canSpeedUp() ? Color.white : Color.gray);
                        break;
                    case "speedDown":
                        setKeyColor(key, Server.canSpeedDown() ? Color.white : Color.gray);
                        break;*/
                    case "play":
                        if (canPlay()) {
                            if (Server.isInputing()) {
                                setKeyColor(key, Color.white);
                            }
                            else {
                                setKeyColor(key, Color.green);
                            }
                        } else {
                            setKeyColor(key, Color.gray);
                        }
                        if (Server.isInputing()) {
                            playKey.GetComponent<RawImage>().texture = stopImage;
                        } else {
                            playKey.GetComponent<RawImage>().texture = playImage;
                        }
                        break;
                    case "accept":
                        if (acceptable()) {
                            setKeyColor(key, Color.green);
                        } else {
                            setKeyColor(key, Color.gray);
                        }
                        break;
                    case "repeat":
                        if (repeatable()) {
                            if (Server.isInputing()) {
                                setKeyColor(key, Color.white);
                            } else {
                                setKeyColor(key, Color.red);
                            }
                        } else {
                            setKeyColor(key, Color.gray);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    bool canPlay() {
        return !Server.isInSession() || Server.isInputing();
    }

    bool acceptable() {
        return Server.isInSession() && !Server.isInputing();
    }

    bool repeatable() {
        return Server.isInSession();
    }

    void setKeyColor(RectTransform key, Color color) {
        var colors = key.GetComponent<Button>().colors;
        colors.normalColor = color;
        key.GetComponent<Button>().colors = colors;
    }

    bool cursorInsideKey(RectTransform key) {
        if (cursor.localPosition.x < key.localPosition.x - key.rect.width * key.localScale.x / 2) return false;
        if (cursor.localPosition.x > key.localPosition.x + key.rect.width * key.localScale.x / 2) return false;
        if (cursor.localPosition.y < key.localPosition.y - key.rect.height * key.localScale.y / 2) return false;
        if (cursor.localPosition.y > key.localPosition.y + key.rect.height * key.localScale.y / 2) return false;
        return true;
    }

    public void confirm() {
        updateHover();

        if (hoverKey != null) {
            switch (hoverKey.name) {
                /*case "normal":
                    Server.setMethod(Server.Method.normal);
                    break;
                case "baseline":
                    Server.setMethod(Server.Method.baseline);
                    break;
                case "headOnly":
                    Server.setMethod(Server.Method.headOnly);
                    break;
                case "dwell":
                    Server.setMethod(Server.Method.dwell);
                    break;
                case "zoonIn":
                    Server.zoomIn();
                    break;
                case "zoonOut":
                    Server.zoomOut();
                    break;
                case "speedUp":
                    Server.speedUp();
                    break;
                case "speedDown":
                    Server.speedDown();
                    break;*/
                case "play":
                    if (canPlay()) {
                        Server.play();
                    }
                    break;
                case "accept":
                    if (acceptable()) {
                        Server.accept();
                    }
                    break;
                case "repeat":
                    if (repeatable()) {
                        Server.repeat();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
