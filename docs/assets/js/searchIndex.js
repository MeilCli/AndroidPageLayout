
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"FirstVisiblePageFloatChangedEventArgs",
        content:"FirstVisiblePageFloatChangedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"Resource Styleable",
        content:"Resource Styleable",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"Resource",
        content:"Resource",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"Resource String",
        content:"Resource String",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"PageLayout LayoutParams",
        content:"PageLayout LayoutParams",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"FirstVisiblePageChangedEventArgs",
        content:"FirstVisiblePageChangedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"PageLayout",
        content:"PageLayout",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"Resource Attribute",
        content:"Resource Attribute",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"LinearStackLayout",
        content:"LinearStackLayout",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"Resource Layout",
        content:"Resource Layout",
        description:'',
        tags:''
    });

    a({
        id:10,
        title:"Resource Id",
        content:"Resource Id",
        description:'',
        tags:''
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/FirstVisiblePageFloatChangedEventArgs',
        title:"FirstVisiblePageFloatChangedEventArgs",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/Styleable',
        title:"Resource.Styleable",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/Resource',
        title:"Resource",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/String',
        title:"Resource.String",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/LayoutParams',
        title:"PageLayout.LayoutParams",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/FirstVisiblePageChangedEventArgs',
        title:"FirstVisiblePageChangedEventArgs",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/PageLayout',
        title:"PageLayout",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/Attribute',
        title:"Resource.Attribute",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/LinearStackLayout',
        title:"LinearStackLayout",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/Layout',
        title:"Resource.Layout",
        description:""
    });

    y({
        url:'/AndroidPageLayout/AndroidPageLayout/api/AndroidPageLayout/Id',
        title:"Resource.Id",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
