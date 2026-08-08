"use client";

import { useCallback, useEffect, useState } from "react";
import { api, ApiError} from "@/lib/api";


export function useApiQuery<T>(path: string | null) {
    const [data, setData] = useState<T | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [reloadkey, setReloadKey] = useState(0);

    useEffect(()=>{
        if(!path){
            setIsLoading(false);
            return;
        }

        let cancelled = false;
        setIsLoading(true);
        setError(null);

        api.get<T>(path).then((result)=>{
            if(!cancelled) setData(result);
        }).catch((err)=>{
            setError(err instanceof ApiError ? err.message : "Failed to load dashboard");
        }).finally(()=>{
            if(!cancelled) setIsLoading(false);
        });

        return () => {
            cancelled = true;
        };
    }, [path, reloadkey]);

    const refetch = useCallback(()=>setReloadKey((k) => k + 1), []);

    return {data, isLoading, error, refetch};
}