def(listlize-part x,n if(or(=(len(x),None),>(n,-(len(x),1))),x,listlize-part(set(x,n,listlize-part(atlist(x,n),1)),+(n,1))))
defn(listlize x listlize-part(atlist(list(x),0),1))
(def noc2list x (listlize (atlist x 0)))
(def evallist l (if (!= l list) l (rename((def evallist-part l temp  
                                                (if (= (len temp) (len l)) 
                                                    temp 
                                                    (evallist-part l (append temp (evallist (at l (len temp))))))) l (append list (at l 0))) call)))
(defn and a b (if a b a))
(defn or a b (if a a b))
(def not a (if a False True))
(def >= a b (or (> a b) (= a b)))
(def <= a b (or (< a b) (= a b)))
(def isempty x (= 0 (len x)))
(def isatom x (= None (len x)))
def(isnosub,x,or(=(0,len(x)),=(None,len(x))))
def(getname,x,rename(list,x))
#(def map f x (if (isempty x) x (insert (f (first x)) (map f (rest x)))))

def(map,f,x,def(map-part,f,x,n,if(=(n,len(x)),x,map-part(f,set(x,n,f(at(x,n))),+(n,1))))(f,x,0))
(def mtree f x (if (isempty x) 
                    x
                    (if (isatom x)
                        (f x)
                        (insert (mtree f (first x)) (mtree f (rest x))))))

def(cond,listofc,lam(condpair,if(at(condpair,0),at(condpair,1),cond(rest(listofc))))(first(listofc)))
def(let,varform,varvalue,fun,append(lam,varform,fun)(varvalue))
defn(iferr,f,handle,lam(x,if(=(x,cerr),handle(x),x))(catch(f)))