var World = {

    init: function initFn() {
        this.createOverlays();
    },

    createOverlays: function createOverlaysFn() {
        /*
            First a AR.TargetCollectionResource is created with the path to the Wikitude Target Collection(.wtc) file.
            This .wtc file can be created from images using the Wikitude Studio. More information on how to create them
            can be found in the documentation in the TargetManagement section.
            Each target in the target collection is identified by its target name. By using this
            target name, it is possible to create an AR.ImageTrackable for every target in the target collection.
         */
        this.targetCollectionResource = new AR.TargetCollectionResource("assets/magazine.wtc", {
            onError: World.onError
        });

        /*
            This resource is then used as parameter to create an AR.ImageTracker. Optional parameters are passed as
            object in the last argument. In this case a callback function for the onTargetsLoaded trigger is set. Once
            the tracker loaded all of its target images this callback function is invoked. We also set the callback
            function for the onError trigger which provides a sting containing a description of the error.
         */
        this.tracker = new AR.ImageTracker(this.targetCollectionResource, {
            onTargetsLoaded: World.showInfoBar,
            onError: World.onError
        });

        /*
            The next step is to create the augmentation. In this example an image resource is created and passed to the
            AR.ImageDrawable. A drawable is a visual component that can be connected to a Trackable
            (AR.ImageTrackable, AR.InstantTrackable or AR.ObjectTrackable) or a geolocated object (AR.GeoObject). The
            AR.ImageDrawable is initialized by the image and its size. Optional parameters allow for transformations
            relative to the recognized target.
        */

        /*
            The button is created similar to the overlay feature. An AR.ImageResource defines the look of the button
            and is reused for both buttons.
        */
        this.imgButton = new AR.ImageResource("assets/wwwButton.jpg", {
            onError: World.onError
        });

        /* Create overlay for page one of the magazine. */
        var imgOne = new AR.ImageResource("assets/imageOne.png", {
            onError: World.onError
        });
        var overlayOne = new AR.ImageDrawable(imgOne, 1, {
            translate: {
                x: -0.15
            }
        });

        /*
            For each target an AR.ImageDrawable for the button is created by utilizing the helper function
            createWwwButton(url, options). The returned drawable is then added to the drawables.cam array on
            creation of the AR.ImageTrackable.
        */
        var pageOneButton = this.createWwwButton("https://www.blue-tomato.com/en-US/products/?q=sup", 0.1, {
            translate: {
                x: -0.25,
                y: -0.25
            },
            zOrder: 1
        });


        /*
            Using an AR.HtmlDrawable it is possible to display HTML content inside the AR scene, the same way images are
            displayed. In this example a weather widget is added on top of the image target to present the real-time
            weather in Maui, Hawaii.

            In general any HTML content can be loaded by passing a relative or absolute URL. Additionally HTML content
            can also be passed as a string; please see the API reference for more details on how to specify the content
            when creating the drawable. This example uses a relative URL to the weather widget that is stored as .html
            file in the example's assets subfolder.

            Once the content has been chosen it is important to think about the viewport the content will need in order
            to be laid out correctly. The viewport is the area that is available to the HTML content when it is
            rendered. It is independent of the actual area the AR.HtmlDrawable will need on screen when placed in the
            AR scene.

            The viewport width and height needs to be set when constructing the AR.HtmlDrawable and should also be set
            as meta tag inside the HTML content that is used. This tells the HTML rendering engine to use the specified
            viewport size during layouting.

            Check out the viewport meta-tag in the weather.html:
            <meta name="viewport" content="target-densitydpi=device-dpi, width = 320, user-scalable = 0">

            Make sure that the value of width is set according to the size (in pixel) the HTML content needs. It should
            also correspond to the viewportWidth value specified during the creation of the AR.HtmlDrawable.

            Similar to viewportWidth the viewportHeight is specified to define the available pixel height during
            rendering of the content. If you are unsure of the pixel size of the HTML content at hand, you can use the
            developer tools built into modern browsers (e.g. WebInspector) to take measure.

            In the code example below we are putting that all together to a working AR.HtmlDrawable. The created
            drawable is added to the list of ImageTrackable drawables just like any other drawable.

            Interaction with an AR.HtmlDrawable is controlled with the clickThroughEnabled and
            allowDocumentLocationChanges properties. Setting clickThroughEnabled will forward click events to the HTML
            content making it possible to follow links or click buttons. If the content of the HTML drawable should not
            change allowDocumentLocationChanges can be set to false so links are not followed. It is still possible to
            react on clicked links by using the onDocumentLocationChanged trigger. The example uses this trigger to
            open clicked links fullscreen in a browser.
        */
        var weatherWidget = new AR.HtmlDrawable({
            uri: "assets/weather.html"
        }, 0.25, {
            viewportWidth: 320,
            viewportHeight: 100,
            backgroundColor: "#FFFFFF",
            translate: {
                x: 0.36,
                y: 0.5
            },
            horizontalAnchor: AR.CONST.HORIZONTAL_ANCHOR.RIGHT,
            verticalAnchor: AR.CONST.VERTICAL_ANCHOR.TOP,
            clickThroughEnabled: true,
            allowDocumentLocationChanges: false,
            onDocumentLocationChanged: function onDocumentLocationChangedFn(uri) {
                AR.context.openInBrowser(uri);
            },
            onError: World.onError
        });

        /*
            This combines everything by creating an AR.ImageTrackable with the previously created tracker,
            the name of the image target and the drawable that should augment the recognized image.

            Important: If you replace the tracker file with your own, make sure to change the target name accordingly.
            Use a specific target name to respond only to a certain target or use a wildcard to respond to any or a
            certain group of targets.
        */
        this.pageOne = new AR.ImageTrackable(this.tracker, "pageOne", {
            drawables: {
                cam: [overlayOne, pageOneButton, weatherWidget]
            },
            onImageRecognized: World.hideInfoBar,
            onError: World.onError
        });
    },

    onError: function onErrorFn(error) {
        alert(error)
    },

    createWwwButton: function createWwwButtonFn(url, size, options) {
        /*
            As the button should be clickable the onClick trigger is defined in the options passed to the
            AR.ImageDrawable. In general each drawable can be made clickable by defining its onClick trigger. The
            function assigned to the click trigger calls AR.context.openInBrowser with the specified URL, which
            opens the URL in the browser.

        */
        options.onClick = function() {
            AR.context.openInBrowser(url);
        };
        return new AR.ImageDrawable(this.imgButton, size, options);
    },

    hideInfoBar: function hideInfoBarFn() {
        document.getElementById("infoBox").style.display = "none";
    },

    showInfoBar: function worldLoadedFn() {
        document.getElementById("infoBox").style.display = "table";
        document.getElementById("loadingMessage").style.display = "none";
    }
};

World.init();