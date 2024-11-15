using System;
using System.Collections.Generic;
using System.Linq;
using static Yogaewonsil.Common.CookMethod;
using static Yogaewonsil.Common.Food;

namespace Yogaewonsil.Common {
    public static class Recipe {

        // 레시피 관리하는 데이터베이스 입니다. 음식이 추가/수정될 경우 `recipeList`를 추가/수정하면 됩니다.
        // (CookMethod, ([input Food1, input Food2, ...], output Food)) 식으로 정의됩니다.
        // 참고: 레시피 구체화.xls from slack by 상언님
        // https://2024fallswppimo.slack.com/archives/C07J19PU63H/p1731530013617699
        private static readonly Tuple<CookMethod, Tuple<Food[], Food>>[] recipeList = 
        {
            // 초밥류
            new(밥짓기, new(new[]{쌀}, 밥)),
            new(합치기, new(new[]{식초, 밥}, 식초밥)),
            // 계란초밥
            new(굽기, new(new[]{계란}, 계란말이)),
            new(초밥제작, new(new[]{계란말이, 식초밥}, 계란초밥)),
            // 유부초밥
            new(손질, new(new[]{두부}, 두부조각)),
            new(튀기기, new(new[]{두부조각}, 유부)),
            new(초밥제작, new(new[]{유부, 식초밥}, 유부초밥)),
            // 광어초밥
            new(손질, new(new[]{광어}, 손질된광어)),
            new(초밥제작, new(new[]{손질된광어, 식초밥}, 광어초밥)),
            // 고등어초밥
            new(손질, new(new[]{고등어}, 손질된고등어)),
            new(초밥제작, new(new[]{손질된고등어, 식초밥}, 고등어초밥)),
            // 소고기초밥
            new(손질, new(new[]{소고기}, 손질된소고기)),
            new(손질, new(new[]{손질된소고기}, 소고기조각)),
            new(굽기, new(new[]{소고기조각}, 구운소고기조각)),
            new(초밥제작, new(new[]{구운소고기조각, 식초밥}, 소고기초밥)),
            // 참치붉은속살초밥 (일반참치)
            new(손질, new(new[]{일반참치}, 손질된일반참치)),
            new(초밥제작, new(new[]{손질된일반참치, 식초밥}, 참치붉은속살초밥)),
            // 연어초밥 (연어->연어필렛->연어조각->연어초밥)
            new(손질, new(new[]{연어}, 연어필렛)),
            new(손질, new(new[]{연어필렛}, 연어조각)),
            new(초밥제작, new(new[]{연어조각, 식초밥}, 연어초밥)),
            // 훈제연어초밥 (연어->연어필렛->연어조각->구운연어조각->훈제연어초밥)
            new(손질, new(new[]{연어}, 연어필렛)),
            new(손질, new(new[]{연어필렛}, 연어조각)),
            new(굽기, new(new[]{연어조각}, 구운연어조각)),
            new(초밥제작, new(new[]{구운연어조각, 식초밥}, 훈제연어초밥)),
            // 장어초밥 (장어->손질된장어->구운장어조각->장어초밥)
            new(손질, new(new[]{장어}, 손질된장어)),
            new(굽기, new(new[]{손질된장어}, 구운장어조각)),
            new(초밥제작, new(new[]{구운장어조각, 식초밥}, 장어초밥)),
            // 참치대뱃살초밥 (고급참치)
            new(손질, new(new[]{고급참치}, 손질된고급참치)),
            new(초밥제작, new(new[]{손질된고급참치, 식초밥}, 참치대뱃살초밥)),

            // 라멘류
            // 라멘육수 (물, 돼지고기, 채소)
            new(끓이기, new(new[]{물, 돼지고기, 채소}, 라멘육수)),
            // 소유라멘 (라멘육수, 생면, 간장, 계란)
            new(끓이기, new(new[]{라멘육수, 생면, 간장, 계란}, 소유라멘)),
            // 미소라멘 (라멘육수, 미소, 생면, 계란)
            new(끓이기, new(new[]{라멘육수, 미소, 생면, 계란}, 미소라멘)),
            // 아부라라멘 (면->삶은면, 돼지고기->손질된돼지고기->구운손질된돼지고기)
            // (삶은면, 구운손질된돼지고기, 간장, 계란)
            new(끓이기, new(new[]{생면}, 삶은면)),
            new(손질, new(new[]{돼지고기}, 손질된돼지고기)),
            new(굽기, new(new[]{손질된돼지고기}, 구운손질된돼지고기)),
            new(비가열조리, new(new[]{삶은면, 구운손질된돼지고기, 간장, 계란}, 아부라라멘)),
            // 돈코츠라멘 (라멘육수, 생면, 차슈 계란)
            new(끓이기, new(new[]{라멘육수, 생면, 차슈, 계란}, 돈코츠라멘)),
            // 츠케멘 (라멘육수, 삶은면, 구운손질된돼지고기, 계란)
            new(비가열조리, new(new[]{라멘육수, 삶은면, 구운손질된돼지고기, 계란}, 츠케멘)),

            // 튀김류
            // 새우튀김 ((새우->손질된새우), 튀김가루)
            new(손질, new(new[]{새우}, 손질된새우)),
            new(튀기기, new(new[]{손질된새우, 튀김가루}, 새우튀김)),
            // 돈가스 (돼지고기->손질된돼지고기->돈가스조각, 밥)
            new(손질, new(new[]{돼지고기}, 손질된돼지고기)),
            new(튀기기, new(new[]{손질된돼지고기}, 돈가스조각)),
            new(비가열조리, new(new[]{돈가스조각, 밥}, 돈가스)),

            // 구이류
            // 일본식함박스테이크 (채소->손질된채소, 손질된돼지고기, 계란)
            new(손질, new(new[]{채소}, 손질된채소)),
            new(굽기, new(new[]{손질된채소, 손질된돼지고기, 계란}, 일본식함박스테이크)),
            // 차슈 (손질된돼지곡, 간장, 물)
            new(굽기, new(new[]{손질된돼지고기, 간장, 물}, 차슈)),
            // 연어스테이크 굽기(연어필렛, 손질된채소)
            new(굽기, new(new[]{연어필렛, 손질된채소}, 연어스테이크)),
            // 와규스테이크 굽기(손질된소고기, 손질된채소)
            new(굽기, new(new[]{손질된소고기, 손질된채소}, 와규스테이크)),

            // 덮밥류
            // 오니기리 비가열조리(밥, (손질된광어 or 손질된고등어 or ... 생선류))
            new(비가열조리, new(new[]{밥, 손질된광어}, 오니기리)),
            new(비가열조리, new(new[]{밥, 손질된고등어}, 오니기리)),
            new(비가열조리, new(new[]{밥, 연어조각}, 오니기리)),
            new(비가열조리, new(new[]{밥, 연어필렛}, 오니기리)),
            new(비가열조리, new(new[]{밥, 손질된일반참치}, 오니기리)),
            new(비가열조리, new(new[]{밥, 손질된장어}, 오니기리)),
            new(비가열조리, new(new[]{밥, 손질된고급참치}, 오니기리)),
            // 에비텐동 비가열조리(밥, 새우튀김 간장)
            new(비가열조리, new(new[]{밥, 새우튀김, 간장}, 에비텐동)),
            // 사바동 비가열조리(굽기(손질된고등어, 손질된채소)->구운고등어, 밥)
            new(굽기, new(new[]{손질된고등어, 손질된채소}, 구운고등어)),
            new(비가열조리, new(new[]{구운고등어, 밥}, 사바동)),
            // 규동 비가열조리(굽기(손질(손질된소고기)->소고기조각)->구운소고기조각), 손질된채소, 계란, 밥)
            new(손질, new(new[]{손질된소고기}, 소고기조각)),
            new(굽기, new(new[]{소고기조각}, 구운소고기조각)),
            new(비가열조리, new(new[]{구운소고기조각, 손질된채소, 계란, 밥}, 규동)),
            // 사케동 (연어조각, 간장, 손질된채소, 밥)
            new(비가열조리, new(new[]{연어조각, 간장, 손질된채소, 밥}, 사케동)),
            // 차슈동 (차슈, 밥, 손질된채소)
            new(비가열조리, new(new[]{차슈, 밥, 손질된채소}, 차슈동)),
            // 연어스테이크덮밥 (연어스테이크, 밥)
            new(비가열조리, new(new[]{연어스테이크, 밥}, 연어스테이크덮밥)),
            // 우나기동 (구운장어조각, 밥)
            new(비가열조리, new(new[]{구운장어조각, 밥}, 우나기동)),

            // 미소국 끓이기(손질(두부)->손질된두부, 미소, 물)
            new(손질, new(new[]{두부}, 손질된두부)),
            new(끓이기, new(new[]{손질된두부, 미소, 물}, 미소국)),
            // 차 (물, 채소)
            new(끓이기, new(new[]{물, 채소}, 차)),
        };

        // 실제로 런타임에는 `recipe`에서 레시피를 찾아옵니다.
        // 위의 `recipeList`를 바탕으로 `recipe`를 초기화합니다.
        private static readonly Dictionary<CookMethod, Dictionary<HashSet<Food>, Food>> recipe =  new (
                from recipe in recipeList
                group recipe by recipe.Item1 into g // CookMethod로 그룹화
                select new KeyValuePair<CookMethod, Dictionary<HashSet<Food>, Food>>(
                    g.Key,
                    new (
                        from recipe in g
                        select new KeyValuePair<HashSet<Food>, Food>(
                            new(recipe.Item2.Item1), // input Food들을 HashSet으로 묶음
                            recipe.Item2.Item2 // output Food
                        )
                    )
                )            
            );

        // 레시피에 존재하는 조리법인지 확인합니다.
        public static bool IsValid(CookMethod cookMethod, HashSet<Food> input) {
            return recipe[cookMethod].ContainsKey(input);
        }
        
        // 레시피에 존재하는 조리법을 실행합니다.
        // 조리법이 존재하지 않는다면 `실패요리`를 반환합니다. 
        // 조리법이 존재하면 해당 조리법을 실행한 결과값 (Food) 를 반환합니다.
        public static Food Execute(CookMethod cookMethod, HashSet<Food> input) {
            if (!IsValid(cookMethod, input)) {
                return 실패요리;
            }
            return recipe[cookMethod][input];
        }
    }  
}
